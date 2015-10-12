namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperation<TResponse> : IOperation where TResponse : IOperationResponse
    {
    }
}