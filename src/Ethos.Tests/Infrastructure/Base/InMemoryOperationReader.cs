using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Tests.Infrastructure.Base
{
    public class InMemoryOperationReader : IOperationReader
    {
        public IOperation ReadOperation(IDictionary<byte, object> parameters)
        {
            throw new System.NotImplementedException();
        }

        public byte ReadPromiseId(IDictionary<byte, object> parameters)
        {
            throw new System.NotImplementedException();
        }

        public IOperationResponse ReadResponse(IDictionary<byte, object> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}