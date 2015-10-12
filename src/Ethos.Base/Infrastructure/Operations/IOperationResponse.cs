namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationResponse
    {
        string ModalErrors { get; }
        bool IsValid { get; }
    }
}