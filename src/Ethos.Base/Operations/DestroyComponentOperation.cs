using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Operations
{
    public class DestroyComponentOperation : IOperation<DestroyComponentResponse>
    {
        public byte ComponentId { get; }

        public DestroyComponentOperation(byte componentId)
        {
            ComponentId = componentId;
        }
    }

    public class DestroyComponentResponse : OperationResponseBase
    {
    }
}