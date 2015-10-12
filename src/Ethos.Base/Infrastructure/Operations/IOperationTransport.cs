using System.Collections.Generic;

namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationTransport
    {
        void SendOperation(OperationCode code, IDictionary<byte, object> parameters);
    }
}