namespace FEZRepacker.Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var version = Core.Metadata.Version;
            // showoff
            Console.WriteLine($"=== {version} ===\n");

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
