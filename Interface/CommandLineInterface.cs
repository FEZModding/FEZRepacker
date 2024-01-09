using FEZRepacker.Interface.Actions;

namespace FEZRepacker.Interface
{
    internal static class CommandLineInterface
    {

        public static CommandLineAction[] Commands = new CommandLineAction[]
        {
            new HelpAction(),
            new ListPackageContentAction(),
            new UnpackConvertAction(),
            new UnpackRawAction(),
            new UnpackDecompressedAction(),
            new PackAction(),
            new UnpackGameAction(),
            new ConvertFromXnbAction(),
            new ConvertToXnbAction()
        };

        public static CommandLineAction? FindCommand(string name)
        {
            var validCommands = Commands.Where(command => command.Name == name || command.Aliases.Contains(name));
            if (validCommands.Any())
            {
                return validCommands.First();
            }
            else
            {
                return null;
            }
        }

        /// <param name="args">The command to execute</param>
        /// <returns>True if a command was executed, false otherwise.</returns>
        public static bool ParseCommandLine(string[] args)
        {
            if (args.Length == 0) return false;

            var command = FindCommand(args[0]);
            if (command == null)
            {
                Console.WriteLine($"Unknown command \"{args[0]}\".");
                Console.WriteLine($"Type \"FEZRepacker.exe --help\" for a list of commands.");
                return false;
            }

            string[] cmdArgs = args.Skip(1).ToArray();

            int maxArgs = command.Arguments.Length;
            int minArgs = command.Arguments.Count(arg => !arg.Optional);

            if (cmdArgs.Length < minArgs || cmdArgs.Length > maxArgs)
            {
                Console.WriteLine($"Invalid usage for command \"{args[0]}\" (incorrect number of parameters).");
                Console.WriteLine($"Use \"FEZRepacker.exe --help {args[0]}\" for a usage instruction for that command.");
                return false;
            }

            try
            {
                command.Execute(cmdArgs);
            }
            catch (Exception ex)
            {
                #if DEBUG
                    throw;
                #else
                    Console.Error.WriteLine($"Error while executing command: {ex.Message}");
                #endif
            }
            return true;
        }

        /// <summary>
        /// Runs interactive mode which repeadetly requests user input and parses it as commands.
        /// </summary>
        public static void RunInteractiveMode()
        {
            Console.Write('\a'); //alert user

            ShowInteractiveModeHelp();

            while (true)
            {
                Console.WriteLine();
                Console.Write("> FEZRepacker.exe ");

                string? line = Console.ReadLine();
                if (line == null) return; // No lines remain to read. Exit the program.

                var args = ParseArguments(line);
                
                if (ParseInteractiveModeCommands(args)) continue;
                if (ParseCommandLine(args)) continue;
                
                ShowInteractiveModeHelp();
                
            }
        }

        private static void ShowInteractiveModeHelp()
        {
            Console.WriteLine("To get usage help, use '--help'");
            Console.WriteLine("To exit, use 'exit'");
        }

        private static bool ParseInteractiveModeCommands(string[] args)
        {
            if (args.Length == 0) return false;

            switch (args[0].ToLower())
            {
                case "cls":
                case "clear":
                    Console.Clear();
                    return true;
                case "exit":
                case "end":
                case "stop":
                case "terminate":
                    Console.WriteLine("Exiting...");
                    return true;
                default:
                    Console.WriteLine();
                    return false;
            }
        }

        private static string[] ParseArguments(string argumentsString)
        {
            return argumentsString.Split('"')
                .SelectMany((element, index) => (index % 2 == 0) ? element.Split(' ') : new[] { element })
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }
    }
}
