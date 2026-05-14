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
                ShowGeneralHelp();
                return;
            }

            var arg = args.GetValueOrDefault(Command, string.Empty);
            var command = CommandLineInterface.FindCommand(arg);
            if (command != null)
            {
                ShowCommandHelp(command);
            }
            else
            {
                Console.WriteLine($"Unknown command \"{arg}\".");
                Console.WriteLine($"Use \"--help\" parameter for a list of commands.");
            }
        }

        private static void ShowGeneralHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            foreach (var command in CommandLineInterface.Commands)
            {
                ShowCommandHelp(command);
                Console.WriteLine();
            }
        }

        private static void ShowCommandHelp(CommandLineAction command)
        {
            Console.Write($"Command: ");
            ShowCommandUsage(command);
            if (command.Aliases.Length > 0)
            {
                Console.WriteLine($"Aliases: {string.Join(", ", command.Aliases)}");
            }

            Console.WriteLine($"Description: {command.Description}");
        }

        public static void ShowCommandUsage(CommandLineAction command)
        {
            var usage = $"{command.Name}";
            foreach (var arg in command.Arguments)
            {
                if (arg.Type == ArgumentType.Flag)
                {
                    usage += arg.Type == ArgumentType.RequiredPositional ? " --" : " [--";
                    usage += arg.Name;
                    if (arg.Aliases.Length > 0)
                    {
                        usage += $" (-{string.Join(" -", arg.Aliases)})";
                    }

                    usage += arg.Type == ArgumentType.RequiredPositional ? "" : "]";
                }
                else
                {
                    usage += arg.Type == ArgumentType.RequiredPositional ? $" {arg.Name}" : $" [{arg.Name}]";
                }
            }

            Console.WriteLine(usage);
        }
    }
}
