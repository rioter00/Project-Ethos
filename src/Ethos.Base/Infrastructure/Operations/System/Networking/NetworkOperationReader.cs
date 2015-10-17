using System.Collections.Generic;
using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Operations.System.Mapping;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations.System.Networking
{
    public class NetworkOperationReader : IOperationReader
    {
        private readonly OperationMap _map;
        private readonly ISerializationService _serializer;

        public NetworkOperationReader(OperationMap map, ISerializationService serializer)
        {
            _map = map;
            _serializer = serializer;
        }

        public IEnumerable<MappedOperation> ReadMappedOperations(IDictionary<byte, object> parameters)
        {
            var mappedOperationsData = (byte[]) parameters[(byte) OperationParameterCode.OperationMapData];
            return (MappedOperation[]) _serializer.ReadObject(typeof (List<MappedOperation>), mappedOperationsData);
        }

        public IOperation ReadOperation(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];
            var operationData = (byte[]) parameters[(byte) OperationParameterCode.OperationData];

            return (IOperation) _serializer.ReadObject(_map.GetMappedOperation(operationId).OperationType, operationData);
        }

        public IOperationResponse ReadResponse(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];
            var operationResponseData = (byte[]) parameters[(byte) OperationParameterCode.OperationResponseData];

            var mappedOperation = _map.GetMappedOperation(operationId);
            var responseType = mappedOperation.OperationType.GetResponseType();

            return (IOperationResponse) _serializer.ReadObject(responseType, operationResponseData);
        }

        public byte ReadPromiseId(IDictionary<byte, object> parameters)
        {
            return (byte) parameters[(byte) OperationParameterCode.OperationPromiseId];
        }
    }
}