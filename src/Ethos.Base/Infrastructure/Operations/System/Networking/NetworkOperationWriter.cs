using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations.System.Mapping;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations.System.Networking
{
    public class NetworkOperationWriter : IOperationWriter
    {
        private readonly OperationMap _map;
        private readonly ISerializationService _serializer;

        private readonly IOperationTransport _transport;

        public NetworkOperationWriter(OperationMap map, ISerializationService serializer, IOperationTransport transport)
        {
            _map = map;
            _serializer = serializer;

            _transport = transport;
        }

        public void WriteContextInitialization(ContextType contextType)
        {
            _transport.SendOperation(OperationCode.SetupContext, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.ContextType] = contextType
            });
        }

        public void WriteOperation(IOperation operation)
        {
            _transport.SendOperation(OperationCode.HandleOperation, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationData] = _serializer.WriteObject(operation.GetType(), operation)
            });
        }

        public void WriteOperationWithResponse(IOperationPromise promise)
        {
            _transport.SendOperation(OperationCode.HandleOperationWithResponse, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(promise.Operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationData] = _serializer.WriteObject(promise.Operation.GetType(), promise.Operation),
                [(byte) OperationParameterCode.OperationPromiseId] = promise.Id
            });
        }

        public void WriteResponse(IOperation operation, IOperationResponse response)
        {
            _transport.SendOperation(OperationCode.HandleResponse, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationResponseData] = _serializer.WriteObject(response.GetType(), response)
            });
        }
    }
}