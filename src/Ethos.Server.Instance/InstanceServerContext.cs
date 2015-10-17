using System.Collections.Generic;
using Autofac;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Operations;
using Ethos.Base.Infrastructure.Operations.System;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Server.Infrastructure;

namespace Ethos.Server.Instance
{
    public class InstanceServerContext : ServerContextBase
    {
        public IServerTransport Transport { get; }
        public OperationSystem OperationSystem { get; }

        public InstanceServerContext(ISerializer serializer, IServerTransport transport) : base(serializer)
        {
            Transport = transport;

            OperationSystem = new OperationSystem(OperationMap, new SerializationService(Serializer), Transport, new OperationHandlerFactory(Container).CreateHandler);
            OperationSystem.Dispatcher.InitializeContext(ContextType.InstanceServer);
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof (InstanceServerContext).Assembly)
                .Where(t => t.IsOperationHandler())
                .As(t => t.GetHandlerInterfaceType());
        }

        public void OnOperationRequest(OperationCode code, IDictionary<byte, object> parameters)
        {
            OperationSystem.Processor.ProcessOperationRequest(code, parameters);
        }
    }
}