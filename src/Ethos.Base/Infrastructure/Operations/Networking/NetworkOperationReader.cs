using System.Collections.Generic;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations.Networking
{
    public class NetworkOperationReader : IOperationReader
    {
        private readonly OperationMap _map;
        private readonly ISerializer _serializer;

        public NetworkOperationReader(OperationMap map, ISerializer serializer)
        {
            _map = map;
            _serializer = serializer;
        }

        public IOperation ReadOperation(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];
            var operationData = (byte[]) parameters[(byte) OperationParameterCode.OperationData];

            return (IOperation) _serializer.DeserializeObject(_map.GetMappedOperation(operationId).OperationType, operationData);
        }

        public byte ReadPromiseId(IDictionary<byte, object> parameters)
        {
            return (byte) parameters[(byte) OperationParameterCode.OperationPromiseId];
        }

        public IOperationResponse ReadResponse(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];
            var operationResponseData = (byte[]) parameters[(byte) OperationParameterCode.OperationResponseData];

            var mappedOperation = _map.GetMappedOperation(operationId);
            var responseType = OperationService.GetResponseType(mappedOperation.OperationType);

            return (IOperationResponse) _serializer.DeserializeObject(responseType, operationResponseData);
        }
    }
}