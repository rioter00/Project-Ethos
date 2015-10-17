using System;
using Ethos.Base.Infrastructure.Components;
using Ethos.Base.Infrastructure.Systems;

namespace Ethos.Server.Infrastructure.Systems
{
    public abstract class ServerSystemBase<TServerComponent, TClientComponent> : ISystem<TServerComponent, TClientComponent>, IInternalSystem
        where TServerComponent : IComponent 
        where TClientComponent : IComponent
    {
        public Type ServerComponentType => typeof (TServerComponent);
        public Type ClientComponentType => typeof (TClientComponent);

        protected ClientContextBase Context { get; private set; }
        protected TClientComponent Client { get; private set; }

        void IInternalSystem.Initialize(ClientContextBase context, object clientProxy)
        {
            Context = context;
            Client = (TClientComponent) clientProxy;
        }

        public virtual void Awake()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}