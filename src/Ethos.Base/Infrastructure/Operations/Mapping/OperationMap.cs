using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ethos.Base.Infrastructure.Operations.Mapping
{
    public class OperationMap
    {
        private readonly MappedOperation[] _mappedOperations;
        private byte _nextAvailableId;

        public IEnumerable<MappedOperation> MappedOperations => _mappedOperations.Take(_nextAvailableId);

        public OperationMap()
        {
            _mappedOperations = new MappedOperation[byte.MaxValue + 1];
            _nextAvailableId = 0;
        }

        public void MapOperationsInAssembly(Assembly assembly)
        {
            foreach (var operationType in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof (IOperation).IsAssignableFrom(t)))
                MapOperation(operationType);
        }

        public MappedOperation MapOperation(Type operationType)
        {
            if (_mappedOperations[_nextAvailableId] != null)
                throw new InvalidOperationException($"Failed to map operation '{operationType}', an operation is already mapped to id {_nextAvailableId}");

            var mappedEvent = new MappedOperation {Id = _nextAvailableId, OperationType = operationType};
            _mappedOperations[_nextAvailableId++] = mappedEvent;

            return mappedEvent;
        }

        public void SyncMappedOperations(IEnumerable<MappedOperation> mappedOperations)
        {
            for (var x = 0; x < _nextAvailableId; x++)
                _mappedOperations[x] = null;

            foreach (var mappedOperation in mappedOperations)
                _mappedOperations[mappedOperation.Id] = mappedOperation;

            _nextAvailableId = (byte) mappedOperations.Count();
        }

        public MappedOperation GetMappedOperation(byte id)
        {
            return _mappedOperations[id];
        }

        public MappedOperation GetMappedOperation(Type operationType)
        {
            for (var x = 0; x < _nextAvailableId; x++)
            {
                if (_mappedOperations[x].OperationType != operationType)
                    continue;

                return _mappedOperations[x];
            }

            throw new InvalidOperationException($"Failed to retrieve mapped operation for type '{operationType}', an operation of that type has not been mapped");
        }

        public bool TryGetMappedOperation(string operationTypeName, out Type operationType)
        {
            for (var x = 0; x < _nextAvailableId; x++)
            {
                if (_mappedOperations[x].OperationType.Name != operationTypeName)
                    continue;

                operationType = _mappedOperations[x].OperationType;
                return true;
            }

            operationType = null;
            return false;
        }
    }
}