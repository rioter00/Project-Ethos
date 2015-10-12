using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations;

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