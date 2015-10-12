namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationWriter
    {
        void WriteOperation(IOperation operation);
        void WriteOperationWithResponse(IOperationPromise promise);
        void WriteResponse(IOperation operation, IOperationResponse response);
    }
}