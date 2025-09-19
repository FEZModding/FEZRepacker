namespace FEZRepacker.Interface
{
    public enum ArgumentType
    {
        RequiredPositional,
        OptionalPositional,
        Flag,
    }

    public struct CommandLineArgument
    {
        public readonly string Name;
        
        public readonly ArgumentType Type;

        public readonly string[] Aliases;

        public readonly string Description;

        public CommandLineArgument(
            string name,
            ArgumentType type = ArgumentType.RequiredPositional,
            string description = "",
            string[]? aliases = null)
        {
            Name = name;
            Type = type;
            Aliases = aliases ?? [];
            Description = description;
        }
    }
}