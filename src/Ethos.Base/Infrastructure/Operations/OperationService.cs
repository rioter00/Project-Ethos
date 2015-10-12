using System;
using System.Linq;

namespace Ethos.Base.Infrastructure.Operations
{
    public class OperationService
    {
        private readonly ActiveOperationsManager _activeOperations;
        private readonly IOperationWriter _writer;

        private readonly Func<Type, IOperationHandler> _handlerFactory;

        public OperationService(ActiveOperationsManager activeOperations, IOperationWriter writer, Func<Type, IOperationHandler> handlerFactory)
        {
            _activeOperations = activeOperations;
            _writer = writer;

            _handlerFactory = handlerFactory;
        }

        public void HandleOperation(IOperation operation)
        {
            var handlerType = typeof (IOperationHandler<>).MakeGenericType(operation.GetType());
            var handler = _handlerFactory(handlerType);

            handlerType.GetMethod("Handle").Invoke(handler, new object[] {operation});
        }

        public void HandleOperationWithResponse(IOperation operation, byte promiseId)
        {
            var responseType = GetResponseType(operation.GetType());

            var handlerType = typeof (IOperationHandler<,>).MakeGenericType(operation.GetType(), responseType);
            var handler = _handlerFactory(handlerType);

            var response = (IOperationResponse) handlerType.GetMethod("Handle").Invoke(handler, new object[] {operation});
            response.PromiseId = promiseId;

            _writer.WriteResponse(operation, response);
        }

        public void HandleResponse(IOperationResponse response)
        {
            var promise = _activeOperations.RetrieveAndRemoveOperation(response.PromiseId);
            typeof (OperationPromise<>).MakeGenericType(response.GetType()).GetMethod("Complete").Invoke(promise, new object[] {response});
        }

        public static Type GetResponseType(Type operationType)
        {
            return operationType.GetInterfaces().Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IOperation<>)).GetGenericArguments()[0];
        }
    }
}