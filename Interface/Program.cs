namespace FEZRepacker.Interface
{
    static class Program
    {
        static int Main(string[] args)
        {
            return args.Length > 0 
                ? CommandLineInterface.ParseCommandLine(args) 
                : CommandLineInterface.RunInteractiveMode();
        }
        
    }
}
