namespace TcpStumps.Examples
{
    using System;

    public static class ConsoleHelper
    {
        public static void ApplicationBanner(string applicationName)
        {
            Console.ForegroundColor = ConsoleColor.White;
            applicationName = "TCP-Stumps Example: " + applicationName;
            Console.WriteLine(applicationName);
            Console.WriteLine(new string('=', applicationName.Length));
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void WaitForExit()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Press [ESC] to Exit.");
            Console.WriteLine();
            Console.ResetColor();

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
            }

            Console.WriteLine("Goodbye!");
        }
    }

}
