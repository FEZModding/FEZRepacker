using System.Globalization;

namespace FEZRepacker.Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // showoff
            Console.WriteLine(
                "================================================\n" +
                "= FEZRepacker 0.4 by Krzyhau & FEZModding Team =\n" +
                "================================================\n"
            );

            // keep number decimals consistent
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

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
