using System;
using System.Collections.Generic;

namespace Ethos.Base.Infrastructure.Operations
{
    public abstract class Operation<TResponse> : IOperation<TResponse> where TResponse : IOperationResponse
    {
        private readonly IList<Action<TResponse>> _callbacks;
        private bool _isCompleted;
        
        protected Operation()
        {
            _callbacks = new List<Action<TResponse>>();
        }

        public void RegisterCallback(Action<TResponse> action)
        {
            if (_isCompleted)
                throw new InvalidOperationException($"Failed to register callback for operation '{GetType()}', the operation has already been completed");

            _callbacks.Add(action);
        }

        public void Complete(TResponse response)
        {
            if (_isCompleted)
                throw new InvalidOperationException($"Failed to complete operation '{GetType()}', the operation may only be completed once");

            foreach (var callback in _callbacks)
                callback(response);

            _isCompleted = true;
        }
    }
}