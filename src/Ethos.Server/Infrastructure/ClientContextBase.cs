using System;
using System.Collections.Generic;
using Autofac;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Server.Infrastructure
{
    public abstract class ClientContextBase : IDisposable
    {
        public ServerContextBase Application { get; }
        public IServerTransport Transport { get; }

        public ILifetimeScope Scope { get; }
        public OperationSystem OperationSystem { get; }

        protected ClientContextBase(ServerContextBase application, IServerTransport transport)
        {
            Application = application;
            Transport = transport;

            Scope = Application.Container.BeginLifetimeScope(this);
            OperationSystem = new OperationSystem(Application.OperationMap, new SerializationService(Application.Serializer), Transport, HandlerFactory);
        }

        public void OnOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            OperationSystem.Processor.ProcessOperationRequest(code, parameters);
        }

        public void Dispose()
        {
            Scope.Dispose();
        }

        private IOperationHandler HandlerFactory(Type type)
        {
            return (IOperationHandler) Scope.Resolve(type);
        }
    }
}