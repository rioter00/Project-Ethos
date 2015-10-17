using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Ethos.Base.Infrastructure;
using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Serialization;
using Ethos.Client.Infrastructure;

namespace Ethos.Client.Console
{
    public class ConsoleClientContext : ClientContextBase
    {
        private volatile bool _isRunning;

        public ConsoleClientContext(ISerializer serializer, IClientTransport transport) : base(serializer, transport)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof (ConsoleClientContext).Assembly)
                .Where(t => t.IsOperationHandler())
                .As(t => t.GetHandlerInterfaceType());
        }

        public override void OnConnect()
        {
            OperationSystem.Dispatcher.InitializeContext(ContextType.ConsoleClient);
        }

        public void Run()
        {
            _isRunning = true;

            Task.Factory.StartNew(() =>
            {
                while (_isRunning)
                {
                    Transport.Service();
                    Thread.Sleep(10);
                }
            },
            TaskCreationOptions.LongRunning);

            while (_isRunning)
            {
                var input = System.Console.ReadLine();

                if (input == null)
                {
                    Console.WriteColor("Please enter a valid command, type '?' or 'help' for a list of available commands", ConsoleColor.Yellow);
                }
                else if (input == "send AuthenticationOperation")
                {
//                    OperationSystem.Dispatcher.DispatchOperation(new AuthenticationOperation {Token = "2C1A506F-F21B-4978-B63E-1EEEC9D776A7"})
//                        .Then(t =>
//                        {
//                            if (!t.IsValid)
//                            {
//                                Console.WriteColor($"Failed to authenticate with the server, {t.ModalErrors}", ConsoleColor.Red);
//                                return;
//                            }
//
//                            Console.WriteColor("Successfully authenticated with the server!", ConsoleColor.Green);
//                        });
//
//                    Console.WriteColor("Successfully sent authentication request", ConsoleColor.Cyan);
                }
                else if (input == "exit")
                {
                    _isRunning = false;
                }
                else
                {
                    Console.WriteColor($"'{input}' was not recognized", ConsoleColor.Red);
                }
            }
        }
    }
}