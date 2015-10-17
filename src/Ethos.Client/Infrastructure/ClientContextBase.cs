using System;
using System.Collections.Generic;
using Autofac;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System;
using Ethos.Base.Infrastructure.Operations.System.Mapping;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Client.Infrastructure
{
    public abstract class ClientContextBase : IDisposable
    {
        public IContainer Container { get; private set; }

        public ISerializer Serializer { get; }
        public IClientTransport Transport { get; }

        public OperationSystem OperationSystem { get; }

        protected ClientContextBase(ISerializer serializer, IClientTransport transport)
        {
            Serializer = serializer;
            Transport = transport;

            OperationSystem = new OperationSystem(new OperationMap(), new SerializationService(serializer), Transport, HandlerFactory);
        }

        public void Setup()
        {
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            Container = builder.Build();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);

        public virtual void OnConnect()
        {
        }

        public virtual void OnOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            OperationSystem.Processor.ProcessOperationRequest(code, parameters);
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        private IOperationHandler HandlerFactory(Type type)
        {
            return (IOperationHandler) Container.Resolve(type);
        }
    }
}