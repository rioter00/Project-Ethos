using Ethos.Server.Infrastructure;

namespace Ethos.Server.Master
{
    public class PlayerClientContext : ClientContextBase
    {
        public PlayerClientContext(ServerContextBase application, IServerTransport transport) : base(application, transport)
        {
        }
    }
}