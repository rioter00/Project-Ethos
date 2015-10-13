using System;
using System.Net;
using Ethos.Base.Infrastructure.Serialization;

namespace Ethos.Client.Console
{
    public class Program
    {
        public ConsoleClientContext Context { get; }

        private Program()
        {
            Context = new ConsoleClientContext(new ProtobufSerializer(), new ConsoleClientTransport(this));
        }

        private static void Main()
        {
            Console.WriteColor("Welcome to the Ethos Client-Console!", ConsoleColor.Green);
            Console.WriteColor("Initializing context...", ConsoleColor.Cyan);

            var program = new Program();
            program.Context.Setup();

            Console.WriteColor("Connecting to server...", ConsoleColor.Cyan);

            program.Context.Transport.Connect(new IPEndPoint(IPAddress.Loopback, 5055));
            program.Context.Run();

            Console.WriteColor("Shutting down...", ConsoleColor.Cyan);
            program.Context.Transport.Disconnect();

            Console.WriteColor("Ethos Client-Console successfully shutdown!", ConsoleColor.Green);
            System.Console.ReadKey();
        }
    }
}