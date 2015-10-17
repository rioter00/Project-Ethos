using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Operations
{
    public class InvokeComponentOperation : IOperation<InvokeComponentResponse>
    {
        public byte ComponentId { get; }
        public byte MethodId { get; }

        public object[] Arguments { get; }

        public InvokeComponentOperation(byte componentId, byte methodId, object[] arguments)
        {
            ComponentId = componentId;
            MethodId = methodId;

            Arguments = arguments;
        }
    }

    public class InvokeComponentResponse : OperationResponseBase
    {
        public object ComponentResponse { get; }

        public InvokeComponentResponse(object componentResponse)
        {
            ComponentResponse = componentResponse;
        }
    }
}