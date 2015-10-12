using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations.Networking
{
    public class NetworkOperationWriter : IOperationWriter
    {
        private readonly OperationMap _map;
        private readonly ISerializer _serializer;

        private readonly IOperationTransport _transport;

        public NetworkOperationWriter(OperationMap map, ISerializer serializer, IOperationTransport transport)
        {
            _map = map;
            _serializer = serializer;

            _transport = transport;
        }

        public void WriteOperation(IOperation operation)
        {
            _transport.SendOperation(OperationCode.HandleOperation, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationData] = _serializer.SerializeObject(operation)
            });
        }

        public void WriteOperationWithResponse(IOperationPromise promise)
        {
            _transport.SendOperation(OperationCode.HandleOperationWithResponse, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(promise.Operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationData] = _serializer.SerializeObject(promise.Operation),
                [(byte) OperationParameterCode.OperationPromiseId] = promise.Id
            });
        }

        public void WriteResponse(IOperation operation, IOperationResponse response)
        {
            _transport.SendOperation(OperationCode.HandleResponse, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationResponseData] = _serializer.SerializeObject(response)
            });
        }
    }
}