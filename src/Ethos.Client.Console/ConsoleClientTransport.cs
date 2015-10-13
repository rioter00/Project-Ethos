using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Client.Infrastructure;
using ExitGames.Client.Photon;
using OperationResponse = ExitGames.Client.Photon.OperationResponse;

namespace Ethos.Client.Console
{
    public class ConsoleClientTransport : IClientTransport, IPhotonPeerListener
    {
        private readonly Program _program;
        private readonly PhotonPeer _peer;

        public ConsoleClientTransport(Program program)
        {
            _program = program;
            _peer = new PhotonPeer(this, ConnectionProtocol.Udp);
        }

        public void Connect(IPEndPoint endPoint)
        {
            _peer.Connect($"{endPoint.Address}:{endPoint.Port}", "Ethos.Master");
        }

        public void Disconnect()
        {
            _peer.Disconnect();
        }

        public void Service()
        {
            _peer.Service();
        }

        public void SendOperation(OperationCode code, Dictionary<byte, object> parameters)
        {
            _peer.OpCustom((byte) code, parameters, true);
        }

        void IPhotonPeerListener.OnEvent(EventData eventData)
        {
            Task.Factory.StartNew(() => _program.Context.OnOperationRequest((OperationCode) eventData.Code, eventData.Parameters));
        }

        void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
        {
            throw new NotImplementedException();
        }

        void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
        {
            if (statusCode == StatusCode.Connect)
                _program.Context.OnConnect();
            if (statusCode == StatusCode.Disconnect)
                _program.Context.OnDisconnect();

            Console.WriteColor($"Network status changed: {statusCode}", ConsoleColor.Cyan);
        }

        void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
        {
            Console.WriteColor($"{level}: {message}", ConsoleColor.Yellow);
        }
    }
}