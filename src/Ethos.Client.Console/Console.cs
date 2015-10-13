using System;

namespace Ethos.Client.Console
{
    public static class Console
    {
        public static void WriteColor(string message, ConsoleColor color)
        {
            var oldColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;

            System.Console.WriteLine(message);
            System.Console.ForegroundColor = oldColor;
        }
    }
}