using System;

namespace Zip
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Arguments can't be empty. Type 'help' if you need list of available commands.");
                Environment.Exit(0);
            }
            Zip zip = new Zip(args);
            switch (args[0])
            {
                case "archive":
                    zip.Archive();
                    break;
                case "update":
                    zip.Update();
                    break;
                case "extract":
                    zip.Extract();
                    break;
                default:
                    throw new ArgumentException($"Command no found - {args[0]}", args[0]);
            }
        }
    }
}
