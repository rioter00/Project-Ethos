namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationPromise
    {
        byte Id { get; }
        IOperation Operation { get; }

        bool IsCompleted { get; }
        IOperationResponse Response { get; }
    }
}