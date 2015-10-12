using System.Collections.Generic;

namespace Ethos.Base.Infrastructure.Operations
{
    public class OperationProcessor
    {
        private readonly OperationService _service;
        private readonly IOperationReader _reader;

        public OperationProcessor(OperationService service, IOperationReader reader)
        {
            _service = service;
            _reader = reader;
        }

        public void ProcessOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            if (code == OperationCode.HandleOperation)
            {
                _service.HandleOperation(_reader.ReadOperation(parameters));
            }
            else if (code == OperationCode.HandleOperationWithResponse)
            {
                _service.HandleOperationWithResponse(_reader.ReadOperation(parameters), _reader.ReadPromiseId(parameters));
            }
            else if (code == OperationCode.HandleResponse)
            {
                _service.HandleResponse(_reader.ReadResponse(parameters));
            }
        }
    }
}