using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Server.Infrastructure;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;

namespace Ethos.Server.Instance
{
    public class ServerPeer : ServerPeerBase, IServerTransport
    {
        private readonly InstanceServerContext _application;

        public ServerPeer(InitResponse initResponse, out InstanceServerContext application) : base(initResponse.Protocol, initResponse.PhotonPeer)
        {
            _application = application = new InstanceServerContext(new BinarySerializer(), this);
            _application.Setup();
        }

        public void SendOperation(OperationCode code, Dictionary<byte, object> parameters)
        {
            SendOperationRequest(new OperationRequest((byte) code, parameters), new SendParameters());
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            _application.OnOperationRequest((OperationCode) eventData.Code, eventData.Parameters);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            _application.Dispose();
        }
    }
}