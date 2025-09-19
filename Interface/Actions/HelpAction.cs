namespace FEZRepacker.Interface.Actions
{
    internal class HelpAction : CommandLineAction
    {
        private const string Command = "command";
        
        public string Name => "--help";
        public string[] Aliases => new[] { "help", "?", "-?", "-h" };
        public string Description => "Displays help for all commands or help for given command.";
        
        public CommandLineArgument[] Arguments => new[] { 
            new CommandLineArgument(Command, ArgumentType.OptionalPositional) 
        };

        public void Execute(Dictionary<string, string> args)
        {
            if (args.Count == 0)
            {
                foreach (var cmd in CommandLineInterface.Commands)
                {
                    ShowHelpFor(cmd);
                    Console.WriteLine();
                }
                return;
            }

            var arg = args.GetValueOrDefault(Command, string.Empty);
            var command = CommandLineInterface.FindCommand(arg);
            if (command != null)
            {
                ShowHelpFor(command);
            }
            else
            {
                Console.WriteLine($"Unknown command \"{arg}\".");
                Console.WriteLine($"Use \"--help\" parameter for a list of commands.");
            }
        }

        private static void ShowHelpFor(CommandLineAction command)
        {
            string allNames = command.Name;
            if (command.Aliases.Length > 0)
            {
                allNames = $"[{command.Name}, {String.Join(", ", command.Aliases)}]";
            }

            var args = command.Arguments.Select(arg =>
            {
                return arg.Type switch
                {
                    ArgumentType.OptionalPositional => $"<{arg.Name}>",
                    ArgumentType.RequiredPositional => $"[{arg.Name}]",
                    ArgumentType.Flag => $"--{arg.Name}",
                    _ => throw new ArgumentOutOfRangeException($"Invalid argument type: {arg.Type}")
                };
            });
            
            Console.WriteLine($"Usage: {allNames} {String.Join(" ", args)}");
            Console.WriteLine($"Description: {command.Description}");
        }
    }
}
