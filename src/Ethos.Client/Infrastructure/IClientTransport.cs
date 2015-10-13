using System.Net;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Client.Infrastructure
{
    public interface IClientTransport : IOperationTransport
    {
        void Connect(IPEndPoint endPoint);
        void Disconnect();

        void Service();
    }
}