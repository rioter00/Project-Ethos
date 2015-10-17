namespace Ethos.Server.Infrastructure.Systems
{
    internal interface IInternalSystem
    {
        void Initialize(ClientContextBase context, object clientProxy);
    }
}