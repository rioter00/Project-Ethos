using Ethos.Server.Infrastructure;

namespace Ethos.Server.Master
{
    public class ConsoleClientContext : ClientContextBase
    {
        public ConsoleClientContext(ServerContextBase application, IServerTransport transport) : base(application, transport)
        {
        }
    }
}