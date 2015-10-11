namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationHandler<in TOperation> : IOperationHandler where TOperation : IOperation
    {
        void Handle(TOperation operation);
    }
}