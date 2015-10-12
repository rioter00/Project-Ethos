using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Server.Infrastructure
{
    public interface IServerTransport : IOperationTransport
    {
        void Disconnect();
    }
}