namespace Ethos.Base.Infrastructure.Operations.System
{
    public class OperationDispatcher
    {
        private readonly ActiveOperationsManager _activeOperations;
        private readonly IOperationWriter _writer;

        public OperationDispatcher(ActiveOperationsManager activeOperations, IOperationWriter writer)
        {
            _activeOperations = activeOperations;
            _writer = writer;
        }

        public void InitializeContext(ContextType contextType)
        {
            _writer.WriteContextInitialization(contextType);
        }

        public void DispatchOperation(IOperation operation)
        {
            _writer.WriteOperation(operation);
        }

        public OperationPromise<TResponse> DispatchOperation<TResponse>(IOperation<TResponse> operation) where TResponse : IOperationResponse
        {
            var promise = _activeOperations.RegisterOperation(operation);
            _writer.WriteOperationWithResponse(promise);

            return promise;
        }
    }
}