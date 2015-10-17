namespace Ethos.Base.Infrastructure.Operations.System
{
    public interface IOperationWriter
    {
        void WriteContextInitialization(ContextType contextType);

        void WriteOperation(IOperation operation);
        void WriteOperationWithResponse(IOperationPromise promise);

        void WriteResponse(IOperation operation, IOperationResponse response);
    }
}