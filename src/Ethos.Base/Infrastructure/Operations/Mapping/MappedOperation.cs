using System;

namespace Ethos.Base.Infrastructure.Operations.Mapping
{
    public class MappedOperation
    {
        public Type OperationType { get; }
        public byte Id { get; }

        public MappedOperation(Type operationType, byte id)
        {
            OperationType = operationType;
            Id = id;
        }
    }
}