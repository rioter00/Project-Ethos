using System.Collections.Generic;
using Autofac;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.Networking;

namespace Ethos.Server.Infrastructure
{
    public abstract class ClientContext
    {
        public ServerContext Application { get; }
        public IServerTransport Transport { get; }

        public ILifetimeScope Scope { get; }

        public ActiveOperationsManager ActiveOperations { get; }

        public OperationDipatcher OperationDipatcher{ get; }
        public OperationProcessor OperationProcessor { get; }

        protected ClientContext(ServerContext application, IServerTransport transport)
        {
            Application = application;
            Transport = transport;

            Scope = Application.Container.BeginLifetimeScope(this);

            ActiveOperations = new ActiveOperationsManager();

            var writer = new NetworkOperationWriter(Application.OperationMap, Application.Serializer, Transport);
            var reader = new NetworkOperationReader(Application.OperationMap, Application.Serializer);

            OperationDipatcher = new OperationDipatcher(ActiveOperations, writer);
            OperationProcessor = new OperationProcessor(new OperationService(ActiveOperations, writer, type => (IOperationHandler) Scope.Resolve(type)), reader);
        }

        public void OnOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            OperationProcessor.ProcessOperationRequest(code, parameters);
        }

        public void OnDisconnect()
        {
            Scope.Dispose();
        }
    }
}