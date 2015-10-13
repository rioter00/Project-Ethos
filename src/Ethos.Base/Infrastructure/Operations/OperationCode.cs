namespace Ethos.Base.Infrastructure.Operations
{
    public enum OperationCode : byte
    {
        SetupContext,
        SyncOperationMap,
        HandleOperation,
        HandleOperationWithResponse,
        HandleResponse
    }
}