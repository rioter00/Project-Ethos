using System.Collections.Generic;
using Autofac;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.Mapping;
using Ethos.Base.Infrastructure.Operations.Networking;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Client.Infrastructure
{
    public abstract class ClientContext
    {
        public ISerializer Serializer { get; }
        public IClientTransport Transport { get; }

        public ActiveOperationsManager ActiveOperationsManager { get; }
        public OperationMap OperationMap { get; }

        public OperationDispatcher OperationDispatcher { get; }
        public OperationProcessor OperationProcessor { get; }

        public IContainer Container { get; private set; }

        protected ClientContext(ISerializer serializer, IClientTransport transport)
        {
            Serializer = serializer;
            Transport = transport;

            ActiveOperationsManager = new ActiveOperationsManager();
            OperationMap = new OperationMap();

            var writer = new NetworkOperationWriter(OperationMap, Serializer, Transport);
            var reader = new NetworkOperationReader(OperationMap, Serializer);

            OperationDispatcher = new OperationDispatcher(ActiveOperationsManager, writer);
            OperationProcessor = new OperationProcessor(new OperationService(ActiveOperationsManager, writer, type => (IOperationHandler) Container.Resolve(type)), reader);
        }

        public void Setup()
        {
            var builder = new ContainerBuilder();
            ConfigureContainer(builder);

            Container = builder.Build();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);

        public void OnOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            if (code == OperationCode.SyncOperationMap)
            {
                var data = (byte[]) parameters[(byte) OperationParameterCode.OperationMapData];
                new OperationMapBinaryFormatter(Serializer).LoadOperationMap(OperationMap, data);
            }
            else
            {
                OperationProcessor.ProcessOperationRequest(code, parameters);
            }
        }

        public virtual void OnConnect()
        {
        }

        public void OnDisconnect()
        {
            Container.Dispose();
        }
    }
}