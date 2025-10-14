using System.CommandLine;

using FEZRepacker.Core.XNB;
using FEZRepacker.Interface.Actions;

namespace FEZRepacker.Interface
{
    internal static class CommandLineInterface
    {
        private static readonly ICommandLineAction[] Actions =
        [
            new ListPackageContentAction(),
            new PackAction(),
            new UnpackAction(),
            new UnpackGameAction(),
            new ConvertFromXnbAction(),
            new ConvertToXnbAction()
        ];

        /// <param name="args">The command to execute</param>
        /// <returns>True if a command was executed, false otherwise.</returns>
        public static int ParseCommandLine(string[] args)
        {
            var version = typeof(XnbSerializer).Assembly.GetName().Version?.ToString() ?? "";
            version = string.Join(".", version.Split('.').Take(3));
            var rootCommand = new RootCommand($"FEZRepacker {version} by Krzyhau & FEZModding Team");

            foreach (var action in Actions)
            {
                var command = new Command(action.Name, action.Description);
                foreach (var alias in action.Aliases)
                {
                    command.Aliases.Add(alias);
                }
                foreach (var argument in action.Arguments)
                {
                    command.Arguments.Add(argument);
                }
                foreach (var option in action.Options)
                {
                    command.Options.Add(option);
                }
                command.SetAction(action.Execute);
                rootCommand.Add(command);
            }
            
            return rootCommand.Parse(args).Invoke();
        }

        /// <summary>
        /// Runs interactive mode which repeatedly requests user input and parses it as commands.
        /// </summary>
        public static int RunInteractiveMode()
        {
            Console.Write('\a'); //alert user
            while (true)
            {
                Console.WriteLine();
                Console.Write("> FEZRepacker.exe ");

                string? line = Console.ReadLine();
                if (line == null) break; // No lines remain to read. Exit the program.

                var args = ParseArguments(line);
                if (ParseInteractiveModeCommands(args, out var shouldTerminate))
                {
                    if (shouldTerminate) break;
                    continue;
                }
                ParseCommandLine(args);
            }

            return 0;
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
                    Console.WriteLine();
                    return false;
            }
        }

        private static string[] ParseArguments(string argumentsString)
        {
            return argumentsString.Split('"')
                .SelectMany((element, index) => (index % 2 == 0) ? element.Split(' ') : [element])
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }
    }
}
