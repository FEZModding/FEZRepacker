using System.Globalization;

using FEZRepacker.Core.XNB;


namespace FEZRepacker.Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string version = typeof(XnbSerializer).Assembly.GetName().Version?.ToString() ?? "";
            version = string.Join(".", version.Split('.').Take(3));
            // showoff
            Console.WriteLine($"=== FEZRepacker {version} by Krzyhau & FEZModding Team ===\n");

            if (args.Length > 0)
            {
                CommandLineInterface.ParseCommandLine(args);
            }
            else
            {
                CommandLineInterface.RunInteractiveMode();
            }
        }
        
    }
}
