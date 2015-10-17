using System;
using System.Collections.Generic;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Server.Infrastructure;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace Ethos.Server.Instance
{
    public class Peer : PeerBase, IServerTransport
    {
        private readonly InstanceServerContext _application;
        private ClientContextBase _context;

        public Peer(InitRequest initRequest, InstanceServerContext application) : base(initRequest)
        {
            _application = application;
        }

        public void SendOperation(OperationCode code, Dictionary<byte, object> parameters)
        {
            SendEvent(new EventData((byte) code, parameters), new SendParameters {Unreliable = false});
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var operationCode = (OperationCode) operationRequest.OperationCode;

            if (_context == null)
            {
                if (operationCode != OperationCode.SetupContext)
                    throw new ArgumentException($"Failed to process operation request '{operationCode}', the context has not been initialized");

                var contextType = (ContextType) operationRequest.Parameters[(byte) OperationParameterCode.ContextType];

                if (contextType == ContextType.PlayerClient)
                    _context = new PlayerClientContext(_application, this);
                else
                    throw new ArgumentException($"Failed setup context type '{contextType}', the context type was not recognized");

                return;
            }

            _context.OnOperationRequest(operationCode, operationRequest.Parameters);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            _context?.Dispose();
        }
    }
}