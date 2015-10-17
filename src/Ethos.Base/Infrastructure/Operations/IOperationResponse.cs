namespace Ethos.Base.Infrastructure.Operations
{
    public interface IOperationResponse
    {
        byte PromiseId { get; set; }

        bool IsValid { get; }
        string ModalErrors { get; }

        void AddModalError(string property, string error);
    }
}