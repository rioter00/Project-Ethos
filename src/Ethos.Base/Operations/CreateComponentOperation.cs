using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Operations
{
    public class CreateComponentOperation : IOperation<CreateComponentResponse>
    {
        public byte ComponentId { get; }

        public CreateComponentOperation(byte componentId)
        {
            ComponentId = componentId;
        }
    }

    public class CreateComponentResponse : OperationResponseBase
    {
    }
}