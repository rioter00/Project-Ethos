using System;
using System.Collections.Generic;
using System.Linq;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations
{
    public class OperationProcessor
    {
        private readonly ISerializer _serializer;
        private readonly OperationMap _map;

        private readonly INetworkTransport _transport;
        private readonly Func<Type, IOperationHandler> _handlerFactory;

        private readonly IDictionary<byte, IOperation> _activeOperations;
        private byte _nextAvailableResponseId;

        public OperationProcessor(ISerializer serializer, OperationMap map, INetworkTransport transport, Func<Type, IOperationHandler> handlerFactory)
        {
            _serializer = serializer;
            _map = map;

            _transport = transport;
            _handlerFactory = handlerFactory;

            _activeOperations = new Dictionary<byte, IOperation>();
            _nextAvailableResponseId = 0;


        }

        public void WriteOperation(IOperation operation)
        {
            _transport.SendOperation(OperationCode.HandleOperation, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationData] = _serializer.SerializeObject(operation)
            });
        }

        public OperationPromise<TResponse> WriteOperationWithResponse<TResponse>(IOperation<TResponse> operation) where TResponse : IOperationResponse
        {
            _transport.SendOperation(OperationCode.HandleOperationWithResponse, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = _map.GetMappedOperation(operation.GetType()).Id,
                [(byte) OperationParameterCode.OperationData] = _serializer.SerializeObject(operation),
                [(byte) OperationParameterCode.OperationResponseId] = _nextAvailableResponseId
            });

            _activeOperations.Add(unchecked(_nextAvailableResponseId++), operation);
            return new OperationPromise<TResponse>(operation);
        }

        public void WriteOperationResponse(byte operationId, byte responseId, IOperationResponse response)
        {
            _transport.SendOperation(OperationCode.HandleOperationResponse, new Dictionary<byte, object>
            {
                [(byte) OperationParameterCode.OperationId] = operationId,
                [(byte) OperationParameterCode.OperationResponseId] = responseId,
                [(byte) OperationParameterCode.OperationResponseData] = _serializer.SerializeObject(response)
            });
        }

        public void ReadOperation(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];
            var operationData = (byte[]) parameters[(byte) OperationParameterCode.OperationData];

            var mappedOperation = _map.GetMappedOperation(operationId);
            var operation = _serializer.DeserializeObject(mappedOperation.OperationType, operationData);

            var handlerType = typeof (IOperationHandler<>).MakeGenericType(mappedOperation.OperationType);
            var handler = _handlerFactory(handlerType);

            handlerType.GetMethod("Handle").Invoke(handler, new[] {operation});
        }

        public void ReadOperationWithResponse(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];
            var operationData = (byte[]) parameters[(byte) OperationParameterCode.OperationData];

            var operationResponseId = (byte) parameters[(byte) OperationParameterCode.OperationResponseId];

            var mappedOperation = _map.GetMappedOperation(operationId);
            var operation = _serializer.DeserializeObject(mappedOperation.OperationType, operationData);

            var responseType = mappedOperation.OperationType.GetInterfaces().Single(t => t == typeof (IOperation<>)).GetGenericArguments()[0];

            var handlerType = typeof (IOperationHandler<,>).MakeGenericType(mappedOperation.OperationType, responseType);
            var handler = _handlerFactory(handlerType);

            var response = (IOperationResponse) handlerType.GetMethod("Handle").Invoke(handler, new[] {operation});
            WriteOperationResponse(operationId, operationResponseId, response);
        }

        public void ReadOperationResponse(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];

            var operationResponseId = (byte) parameters[(byte) OperationParameterCode.OperationResponseId];
            var operationResponseData = (byte[]) parameters[(byte) OperationParameterCode.OperationResponseData];

            var mappedOperation = _map.GetMappedOperation(operationId);
            var responseType = mappedOperation.OperationType.GetInterfaces().Single(t => t == typeof (IOperation<>)).GetGenericArguments()[0];

            var response = (IOperationResponse) _serializer.DeserializeObject(responseType, operationResponseData);

            IOperation operation;
            if (!_activeOperations.TryGetValue(operationResponseId, out operation))
                throw new InvalidOperationException($"Failed to handle event response, no active event exists with id {operationResponseId}");

            ((IOperation<IOperationResponse>) operation).Complete(response);
            _activeOperations.Remove(operationResponseId);
        }
    }
}