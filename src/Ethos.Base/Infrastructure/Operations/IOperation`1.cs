using System;

namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperation<TResponse> : IOperation where TResponse : IOperationResponse
    {
        void RegisterCallback(Action<TResponse> action);
        void Complete(TResponse response);
    }
}