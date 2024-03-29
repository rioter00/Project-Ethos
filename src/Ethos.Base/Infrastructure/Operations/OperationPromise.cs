﻿using System;
using System.Collections.Generic;

namespace Ethos.Base.Infrastructure.Operations
{
    public class OperationPromise<TResponse> : IOperationPromise where TResponse : IOperationResponse
    {
        private readonly IList<Action<TResponse>> _callbacks;

        public byte Id { get; }
        public IOperation Operation { get; }

        public bool IsCompleted { get; private set; }
        public IOperationResponse Response { get; private set; }

        public IEnumerable<Action<TResponse>> Callbacks => _callbacks;

        public OperationPromise(byte id, IOperation operation)
        {
            Id = id;
            Operation = operation;

            _callbacks = new List<Action<TResponse>>();
        }

        public OperationPromise<TResponse> Then(Action<TResponse> action)
        {
            if (IsCompleted)
                throw new InvalidOperationException($"Failed to register callback for operation '{GetType()}', the operation has already been completed");

            _callbacks.Add(action);
            return this;
        }

        public void Complete(TResponse response)
        {
            if (IsCompleted)
                throw new InvalidOperationException($"Failed to complete operation '{GetType()}', the operation may only be completed once");

            foreach (var callback in _callbacks)
                callback(response);

            Response = response;
            IsCompleted = true;
        }
    }
}