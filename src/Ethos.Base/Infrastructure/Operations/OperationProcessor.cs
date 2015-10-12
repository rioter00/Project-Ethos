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

        private readonly IDictionary<byte, IOperationPromise> _activeOperations;
        private byte _nextAvailableResponseId;

        public IEnumerable<IOperation> ActiveOperations => _activeOperations.Values.Select(t => t.Operation);

        public OperationProcessor(ISerializer serializer, OperationMap map, INetworkTransport transport, Func<Type, IOperationHandler> handlerFactory)
        {
            _serializer = serializer;
            _map = map;

            _transport = transport;
            _handlerFactory = handlerFactory;

            _activeOperations = new Dictionary<byte, IOperationPromise>();
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

            var promise = new OperationPromise<TResponse>(operation);
            _activeOperations.Add(unchecked (_nextAvailableResponseId++), promise);

            return promise;
        }

        public void WriteResponse(byte operationId, byte responseId, IOperationResponse response)
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

            var responseType = GetResponseType(mappedOperation.OperationType);

            var handlerType = typeof (IOperationHandler<,>).MakeGenericType(mappedOperation.OperationType, responseType);
            var handler = _handlerFactory(handlerType);

            var response = (IOperationResponse) handlerType.GetMethod("Handle").Invoke(handler, new[] {operation});
            WriteResponse(operationId, operationResponseId, response);
        }

        public void ReadResponse(IDictionary<byte, object> parameters)
        {
            var operationId = (byte) parameters[(byte) OperationParameterCode.OperationId];

            var operationResponseId = (byte) parameters[(byte) OperationParameterCode.OperationResponseId];
            var operationResponseData = (byte[]) parameters[(byte) OperationParameterCode.OperationResponseData];

            var mappedOperation = _map.GetMappedOperation(operationId);
            var responseType = GetResponseType(mappedOperation.OperationType);

            var response = _serializer.DeserializeObject(responseType, operationResponseData);

            IOperationPromise promise;
            if (!_activeOperations.TryGetValue(operationResponseId, out promise))
                throw new InvalidOperationException($"Failed to handle event response, no active event exists with id {operationResponseId}");

            typeof (OperationPromise<>).MakeGenericType(responseType).GetMethod("Complete").Invoke(promise, new[] {response});
            _activeOperations.Remove(operationResponseId);
        }

        private Type GetResponseType(Type operationType)
        {
            return operationType.GetInterfaces().Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IOperation<>)).GetGenericArguments()[0];
        }
    }
}