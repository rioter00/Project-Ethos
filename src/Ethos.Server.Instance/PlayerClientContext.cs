using Ethos.Server.Infrastructure;

namespace Ethos.Server.Instance
{
    public class PlayerClientContext : ClientContextBase
    {
        public PlayerClientContext(ServerContextBase application, IServerTransport transport) : base(application, transport)
        {
        }
    }
}