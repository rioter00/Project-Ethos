using System;

namespace Ethos.Base.Infrastructure.Operations
{
    public class OperationPromise<TResponse> where TResponse : IOperationResponse
    {
        private readonly IOperation<TResponse> _operation;

        public OperationPromise(IOperation<TResponse> operation)
        {
            _operation = operation;
        }

        public OperationPromise<TResponse> Then(Action<TResponse> action)
        {
            _operation.RegisterCallback(action);
            return this;
        }
    }
}