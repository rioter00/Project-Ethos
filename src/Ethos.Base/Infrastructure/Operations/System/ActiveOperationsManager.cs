using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethos.Base.Infrastructure.Operations.System
{
    public class ActiveOperationsManager
    {
        private readonly IDictionary<byte, IOperationPromise> _activeOperations;
        private byte _nextAvailableId;

        public IEnumerable<IOperation> ActiveOperations => _activeOperations.Values.Select(t => t.Operation);

        public ActiveOperationsManager()
        {
            _activeOperations = new Dictionary<byte, IOperationPromise>();
        }

        public OperationPromise<TResponse> RegisterOperation<TResponse>(IOperation<TResponse> operation) where TResponse : IOperationResponse
        {
            var promise = new OperationPromise<TResponse>(unchecked (_nextAvailableId++), operation);
            _activeOperations.Add(promise.Id, promise);

            return promise;
        }

        public IOperationPromise RetrieveAndRemoveOperation(byte promiseId)
        {
            IOperationPromise promise;
            if (!_activeOperations.TryGetValue(promiseId, out promise))
                throw new InvalidOperationException($"Failed to complete active operation, an operation with promise id {promiseId} was not found");

            _activeOperations.Remove(promiseId);
            return promise;
        }
    }
}