using System.Collections.Generic;
using Autofac;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Server.Infrastructure
{
    public abstract class ClientContext
    {
        public ServerContext Application { get; }
        public INetworkTransport Transport { get; }

        public ILifetimeScope Scope { get; }
        public OperationProcessor OperationProcessor { get; }

        protected ClientContext(ServerContext application, INetworkTransport transport)
        {
            Application = application;
            Transport = transport;

            Scope = Application.Container.BeginLifetimeScope(this);
            OperationProcessor = new OperationProcessor(Application.Serializer, Application.OperationMap, Transport, t => (IOperationHandler) Scope.Resolve(t));
        }

        public void OnOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            if (code == OperationCode.HandleOperation)
            {
                OperationProcessor.ReadOperation(parameters);
            }
            else if (code == OperationCode.HandleOperationWithResponse)
            {
                OperationProcessor.ReadOperationWithResponse(parameters);
            }
            else if (code == OperationCode.HandleOperationResponse)
            {
                OperationProcessor.ReadOperationResponse(parameters);
            }
        }

        public void OnDisconnect()
        {
            Scope.Dispose();
        }
    }
}