using FEZRepacker.Interface.Actions;

namespace FEZRepacker.Interface
{
    internal static class CommandLineInterface
    {
        public static readonly CommandLineAction[] Commands =
        [
            new HelpAction(),
            new ListPackageContentAction(),
            new UnpackConvertAction(),
            new UnpackRawAction(),
            new UnpackDecompressedAction(),
            new PackAction(),
            new UnpackGameAction(),
            new ConvertFromXnbAction(),
            new ConvertToXnbAction()
        ];

        public static CommandLineAction? FindCommand(string name)
        {
            return Commands.FirstOrDefault(command => command.Name == name || command.Aliases.Contains(name));
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

            var parsedArgs = ParseArguments(command, args.Skip(1).ToArray());
            if (parsedArgs == null)
            {
                Console.WriteLine($"Invalid usage for command \"{args[0]}\".");
                Console.WriteLine(
                    $"Use \"FEZRepacker.exe --help {args[0]}\" for a usage instruction for that command.");
                return false;
            }

            try
            {
                command.Execute(parsedArgs);
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
        /// Runs interactive mode which repeatedly requests user input and parses it as commands.
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

                if (ParseInteractiveModeCommands(args, out var shouldTerminate))
                {
                    if (shouldTerminate) break;
                    continue;
                }

                if (args.Length > 0)
                {
                    // Handle help command specifically
                    if (args[0].ToLower() == "help" || args[0] == "--help" || args[0] == "-h")
                    {
                        if (args.Length > 1)
                        {
                            // Show help for a specific command
                            var command = FindCommand(args[1]);
                            if (command != null)
                            {
                                ShowCommandHelp(command);
                            }
                            else
                            {
                                Console.WriteLine($"Unknown command \"{args[1]}\".");
                            }
                        }
                        else
                        {
                            ShowGeneralHelp();
                        }

                        continue;
                    }

                    if (ParseCommandLine(args))
                    {
                        continue;
                    }
                }

                ShowInteractiveModeHelp();
            }
        }

        private static void ShowInteractiveModeHelp()
        {
            Console.WriteLine("To get usage help, use 'help' or '--help'");
            Console.WriteLine("For help with a specific command, use 'help <command>'");
            Console.WriteLine("To exit, use 'exit'");
        }

        private static void ShowGeneralHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            foreach (var command in Commands)
            {
                Console.WriteLine($"  {command.Name} - {command.Description}");
                if (command.Aliases.Length < 1) continue;
                Console.WriteLine($"    Aliases: {string.Join(", ", command.Aliases)}");
                Console.WriteLine();
            }
        }

        private static void ShowCommandHelp(CommandLineAction command)
        {
            Console.WriteLine($"Command: {command.Name}");
            if (command.Aliases.Length > 0)
            {
                Console.WriteLine($"Aliases: {string.Join(", ", command.Aliases)}");
            }

            Console.WriteLine($"Description: {command.Description}");
            Console.WriteLine("Usage:");

            var usage = $"FEZRepacker.exe {command.Name}";
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

            if (command.Arguments.Length <= 0) return;

            Console.WriteLine("Arguments:");
            foreach (var arg in command.Arguments)
            {
                var argInfo = $"  {arg.Name}";
                if (arg is { Type: ArgumentType.Flag, Aliases.Length: > 0 })
                {
                    argInfo += $" (Aliases: {string.Join(", ", arg.Aliases.Select(a => $"-{a}"))})";
                }

                argInfo += arg.Type == ArgumentType.RequiredPositional ? " (Required)" : " (Optional)";
                if (!string.IsNullOrEmpty(arg.Description))
                {
                    argInfo += $": {arg.Description}";
                }

                Console.WriteLine(argInfo);
            }
        }

        private static bool ParseInteractiveModeCommands(string[] args, out bool terminationRequested)
        {
            terminationRequested = false;
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
                    terminationRequested = true;
                    Console.WriteLine("Exiting...");
                    return true;
                default:
                    return false;
            }
        }

        private static Dictionary<string, string>? ParseArguments(CommandLineAction command, string[] args)
        {
            var result = new Dictionary<string, string>();
            var positionalArgs = new List<string>();
            var flags = new HashSet<string>();

            // Get all argument definitions for this command
            var requiredPositional = command.Arguments
                .Where(a => a.Type == ArgumentType.RequiredPositional)
                .ToArray();

            var optionalPositional = command.Arguments
                .Where(a => a.Type == ArgumentType.OptionalPositional)
                .ToArray();

            var flagArguments = command.Arguments
                .Where(a => a.Type == ArgumentType.Flag)
                .ToArray();

            // Create a set of all valid flag names and aliases
            var allFlagNames = flagArguments
                .SelectMany(f => new[] { f.Name }.Concat(f.Aliases))
                .ToHashSet();

            // Parse arguments in order, respecting the command's argument definitions
            foreach (var arg in args)
            {
                if (!(arg.StartsWith("--") || arg.StartsWith("-")))
                {
                    positionalArgs.Add(arg);
                    continue;
                }

                string flagContent;
                if (arg.StartsWith("--"))   // Long flag
                {
                    flagContent = arg[2..];
                }
                else if (arg.StartsWith("-"))   // Short flag
                {
                    flagContent = arg[1..];
                }
                else
                {
                    positionalArgs.Add(arg);
                    continue;
                }
                
                if (flagContent.Contains('='))
                {
                    var parts = flagContent.Split('=', 2);
                    if (allFlagNames.Contains(parts[0]))
                    {
                        result[parts[0]] = parts[1];
                    }
                    else
                    {
                        positionalArgs.Add(arg);
                    }
                }
                else if (allFlagNames.Contains(flagContent))
                {
                    flags.Add(flagContent);
                }
                else
                {
                    positionalArgs.Add(arg);
                }
            }

            // Not enough required positional arguments
            if (positionalArgs.Count < requiredPositional.Length)
                return null;

            // Too many positional arguments
            if (positionalArgs.Count > requiredPositional.Length + optionalPositional.Length)
                return null;

            for (int i = 0; i < requiredPositional.Length; i++)
            {
                result[requiredPositional[i].Name] = positionalArgs[i];
            }

            for (int i = 0; i < optionalPositional.Length; i++)
            {
                if (i + requiredPositional.Length < positionalArgs.Count)
                {
                    result[optionalPositional[i].Name] = positionalArgs[i + requiredPositional.Length];
                }
            }

            foreach (var flagArg in command.Arguments.Where(a => a.Type == ArgumentType.Flag))
            {
                if (flags.Contains(flagArg.Name) || flagArg.Aliases.Any(flags.Contains))
                {
                    result[flagArg.Name] = "true";
                }
            }

            return result;
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