using System.Collections.Generic;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System;

namespace Ethos.Tests.Infrastructure.Base
{
    public class InMemoryOperationWriter : IOperationWriter
    {
        private readonly IList<IOperationResponse> _writtenResponses;

        public IEnumerable<IOperationResponse> WrittenResponses => _writtenResponses;

        public InMemoryOperationWriter()
        {
            _writtenResponses = new List<IOperationResponse>();
        }

        public void WriteContextInitialization(ContextType contextType)
        {
            throw new System.NotImplementedException();
        }

        public void WriteMappedOperations()
        {
            throw new System.NotImplementedException();
        }

        public void WriteOperation(IOperation operation)
        {
            throw new System.NotImplementedException();
        }

        public void WriteOperationWithResponse(IOperationPromise promise)
        {
            throw new System.NotImplementedException();
        }

        public void WriteResponse(IOperation operation, IOperationResponse response)
        {
            _writtenResponses.Add(response);
        }
    }
}