using System.Collections.Generic;
using System.Net;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Infrastructure
{
    public interface INetworkTransport
    {
        IPEndPoint EndPoint { get; }

        void SendOperation(OperationCode code, IDictionary<byte, object> parameters);
        void Disconnect();
    }
}