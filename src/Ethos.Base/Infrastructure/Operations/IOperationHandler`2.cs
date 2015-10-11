namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationHandler<in TOperation, out TResponse> : IOperationHandler
        where TOperation : IOperation 
        where TResponse : IOperationResponse
    {
        TResponse Handle(TOperation operation);
    }
}