using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Infrastructure
{
    public interface INetworkTransport
    {
        void SendOperation(OperationCode code, IDictionary<byte, object> parameters);
        void Disconnect();
    }
}