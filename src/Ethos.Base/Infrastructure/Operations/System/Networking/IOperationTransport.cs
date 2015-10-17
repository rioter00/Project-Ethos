using System.Collections.Generic;

namespace Ethos.Base.Infrastructure.Operations.System.Networking
{
    public interface IOperationTransport
    {
        void SendOperation(OperationCode code, Dictionary<byte, object> parameters);
    }
}