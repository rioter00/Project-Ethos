using System;
using Autofac;
using Ethos.Base.Infrastructure.Components;
using Ethos.Base.Infrastructure.Operations.System.Mapping;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Base.Operations;

namespace Ethos.Server.Infrastructure
{
    public abstract class ServerContextBase : IDisposable
    {
        public ISerializer Serializer { get; }
        public IContainer Container { get; private set; }

        public OperationMap OperationMap { get; }
        public ComponentMap OperationsComponentMap { get; }

        protected ServerContextBase(ISerializer serializer)
        {
            Serializer = serializer;

            OperationMap = new OperationMap();
            OperationsComponentMap = new ComponentMap();
        }

        public void Setup()
        {
            OperationMap.MapOperationsInAssembly(typeof (CreateComponentOperation).Assembly);

            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            Container = builder.Build();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);

        public virtual void Dispose()
        {
            Container.Dispose();
        }
    }
}