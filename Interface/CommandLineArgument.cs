namespace FEZRepacker.Interface
{
    public struct CommandLineArgument
    {
        public string Name;
        public bool Optional;
        public CommandLineArgument(string name, bool optional = false)
        {
            Name = name;
            Optional = optional;
        }
    }
}
