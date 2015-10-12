namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationPromise
    {
        IOperation Operation { get; }
        bool IsCompleted { get; }
    }
}