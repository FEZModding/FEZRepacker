namespace FEZRepacker.Interface.Actions
{
    internal class HelpAction : CommandLineAction
    {
        public string Name => "--help";
        public string[] Aliases => new[] { "help", "?", "-?", "-h" };
        public string Description => "Displays help for all commands or help for given command.";
        public CommandLineArgument[] Arguments => new[] { 
            new CommandLineArgument("command", true) 
        };

        public void Execute(string[] args)
        {
            if(args.Length == 0)
            {
                foreach (var cmd in CommandLineInterface.Commands)
                {
                    ShowHelpFor(cmd);
                    Console.WriteLine();
                }
                return;
            }

            var command = CommandLineInterface.FindCommand(args[0]);
            if (command != null)
            {
                ShowHelpFor(command);
            }
            else
            {
                Console.WriteLine($"Unknown command \"{args[0]}\".");
                Console.WriteLine($"Use \"--help\" parameter for a list of commands.");
            }
        }

        private void ShowHelpFor(CommandLineAction command)
        {
            string allNames = command.Name;
            if (command.Aliases.Length > 0)
            {
                allNames = $"[{command.Name}, {String.Join(", ", command.Aliases)}]";
            }

            string argsStr = String.Join(" ", command.Arguments.Select(arg => arg.Optional ? $"<{arg.Name}>" : $"[{arg.Name}]"));

            Console.WriteLine($"Usage: {allNames} {argsStr}");
            Console.WriteLine($"Description: {command.Description}");
        }
    }
}
