using Ethos.Base.Infrastructure.Operations.System.Networking;

namespace Ethos.Server.Infrastructure
{
    public interface IServerTransport : IOperationTransport
    {
        void Disconnect();
    }
}