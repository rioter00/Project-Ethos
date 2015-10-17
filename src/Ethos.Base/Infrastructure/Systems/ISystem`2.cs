using Ethos.Base.Infrastructure.Components;

namespace Ethos.Base.Infrastructure.Systems
{
    public interface ISystem<TServerComponent, TClientComponent> : ISystem
        where TServerComponent : IComponent 
        where TClientComponent : IComponent
    {
    }
}