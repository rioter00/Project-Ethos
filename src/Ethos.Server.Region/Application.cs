using Photon.SocketServer;

namespace Ethos.Server.Region
{
    public class Application : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new Peer(initRequest);
        }

        protected override void Setup()
        {
        }

        protected override void TearDown()
        {
        }
    }
}