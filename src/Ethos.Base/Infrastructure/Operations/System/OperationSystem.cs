using System;
using Ethos.Base.Infrastructure.Operations.System.Mapping;
using Ethos.Base.Infrastructure.Operations.System.Networking;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Base.Infrastructure.Operations.System
{
    public class OperationSystem
    {
        public OperationDispatcher Dispatcher { get; }
        public OperationProcessor Processor { get; }

        public OperationSystem(OperationMap map, ISerializationService serializer, IOperationTransport transport, Func<Type, IOperationHandler> handlerFactory)
        {
            var writer = new NetworkOperationWriter(map, serializer, transport);
            var reader = new NetworkOperationReader(map, serializer);

            var activeOperations = new ActiveOperationsManager();

            Dispatcher = new OperationDispatcher(activeOperations, writer);
            Processor = new OperationProcessor(new OperationService(activeOperations, writer, handlerFactory), reader);
        }
    }
}