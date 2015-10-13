using System;
using Autofac;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Base.Operations;

namespace Ethos.Server.Infrastructure
{
    public abstract class ServerContext : IDisposable
    {
        public ISerializer Serializer { get; }
        public OperationMap OperationMap { get; }

        public IContainer Container { get; private set; }

        protected ServerContext(ISerializer serializer)
        {
            Serializer = serializer;
            OperationMap = new OperationMap();
        }

        public void Setup()
        {
            OperationMap.MapOperationsInAssembly(typeof (AuthenticationOperation).Assembly);

            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            Container = builder.Build();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}