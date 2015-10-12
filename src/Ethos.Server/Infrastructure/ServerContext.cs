using Autofac;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Base.Operations;

namespace Ethos.Server.Infrastructure
{
    public abstract class ServerContext
    {
        public ISerializer Serializer { get; }
        public IContainer Container { get; }

        public OperationMap OperationMap { get; }

        protected ServerContext(ISerializer serializer)
        {
            Serializer = serializer;
            
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            Container = builder.Build();

            OperationMap = new OperationMap();
            OperationMap.MapOperationsInAssembly(typeof (AuthenticationOperation).Assembly);
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);
    }
}