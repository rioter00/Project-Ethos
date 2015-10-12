using System.Collections.Generic;

namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationReader
    {
        IOperation ReadOperation(IDictionary<byte, object> parameters);
        byte ReadPromiseId(IDictionary<byte, object> parameters);
        IOperationResponse ReadResponse(IDictionary<byte, object> parameters);
    }
}