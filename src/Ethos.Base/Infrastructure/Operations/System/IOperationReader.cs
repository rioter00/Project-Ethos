using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations.System.Mapping;

namespace Ethos.Base.Infrastructure.Operations.System
{
    public interface IOperationReader
    {
        IEnumerable<MappedOperation> ReadMappedOperations(IDictionary<byte, object> parameters);

        IOperation ReadOperation(IDictionary<byte, object> parameters);
        IOperationResponse ReadResponse(IDictionary<byte, object> parameters);

        byte ReadPromiseId(IDictionary<byte, object> parameters);
    }
}