namespace FEZRepacker.Interface
{
    internal interface CommandLineAction
    {
        public string Name { get; }
        public string[] Aliases { get; }
        public string Description { get; }
        public CommandLineArgument[] Arguments { get; }
        public void Execute(Dictionary<string, string> args);

    }
}
