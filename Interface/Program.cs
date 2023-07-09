using System.Globalization;

namespace FEZRepacker.Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // showoff
            Console.WriteLine(
                "==============================\n" +
                "= FEZRepacker 0.3 by Krzyhau =\n" +
                "==============================\n"
            );

            // keep number decimals consistent
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            if (args.Length > 0)
            {
                CommandLine.ParseCommandLine(args);
            }
            else
            {
                Console.WriteLine("In order to get usage help, use '--help' argument.\n");
                // TODO: run user interface here?
            }
        }
    }
}
