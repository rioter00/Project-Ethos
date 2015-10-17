using Ethos.Server.Infrastructure;

namespace Ethos.Server.Master
{
    public class RegionClientContext : ClientContextBase
    {
        public RegionClientContext(ServerContextBase application, IServerTransport transport) : base(application, transport)
        {
        }
    }
}