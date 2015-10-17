using Ethos.Server.Infrastructure;

namespace Ethos.Server.Master
{
    public class InstanceClientContext : ClientContextBase
    {
        public InstanceClientContext(ServerContextBase application, IServerTransport transport) : base(application, transport)
        {
        }
    }
}