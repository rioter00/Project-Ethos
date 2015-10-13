using Ethos.Server.Infrastructure;

namespace Ethos.Server.Master
{
    public class PlayerClientContext : ClientContext
    {
        public PlayerClientContext(ServerContext application, IServerTransport transport) : base(application, transport)
        {
        }
    }
}