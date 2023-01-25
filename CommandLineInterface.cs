using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FEZRepacker
{
    internal static partial class CommandLine
    {
        public struct Argument
        {
            public string Name;
            public bool Optional;

            public Argument(string name, bool optional = false)
            {
                Name = name;
                Optional = optional;
            }
        }

        public struct Command
        {
            public string Name;
            public string HelpText;
            public string[] Aliases;
            public Argument[] Arguments;
            public Action<string[]>? Operation;

            public Command()
            {
                Name = "";
                HelpText = "";
                Aliases = new string[0];
                Arguments = new Argument[0];
                Operation = null;
            }
        }

        public static Command[] Commands = new Command[]
        {
            new Command
            {
                Name = "--help", Aliases = new[]{"help", "?", "-?", "-h"},
                HelpText = "Displays help for all commands or help for given command.",
                Arguments = new[]{ new Argument("command", true) },
                Operation = CommandHelp
            },
            new Command
            {
                Name = "--unpack", Aliases = new[]{"-u"},
                HelpText = "Unpacks entire .PAK package into specified directory (creates one if doesn't exist) " +
                "and attempts to convert XNB assets into their corresponding format in the process.",
                Arguments = new[]{ new Argument("pak-path"), new Argument("destination-folder") },
                Operation = delegate(string[] args){ UnpackPackage(args[0], args[1], UnpackingMode.Converted); }
            },
            new Command
            {
                Name = "--unpack-raw",
                HelpText = "Unpacks entire .PAK package into specified directory (creates one if doesn't exist) " +
                "leaving XNB assets in their original form.",
                Arguments = new[]{ new Argument("pak-path"), new Argument("destination-folder") },
                Operation = delegate(string[] args){ UnpackPackage(args[0], args[1], UnpackingMode.Raw); }
            },
            new Command
            {
                Name = "--unpack-decompressed",
                HelpText = "Unpacks entire .PAK package into specified directory (creates one if doesn't exist)." +
                "and attempts to decompress all XNB assets, but does not convert them.",
                Arguments = new[]{ new Argument("pak-path"), new Argument("destination-folder") },
                Operation = delegate(string[] args){ UnpackPackage(args[0], args[1], UnpackingMode.DecompressedXNB); }
            },
            new Command
            {
                Name = "--unpack-fez-content", Aliases = new[]{"-g"},
                HelpText = "Unpacks and converts all game assets into specified directory (creates one if doesn't exist).",
                Arguments = new[]{ new Argument("fez-content-directory"), new Argument("destination-folder") },
                Operation = delegate(string[] args){ UnpackGameAssets(args[0], args[1], UnpackingMode.Converted); }
            },
            new Command
            {
                Name = "--pack", Aliases = new[]{"-p"},
                HelpText = "Loads files from given input directory path, tries to deconvert them and pack into a destination " +
                ".PAK file with given path. If include .PAK path is provided, it'll add its content into the new .PAK package.",
                Arguments = new[]{ new Argument("input-directory-path"), new Argument("destination-pak-path"), new Argument("include-pak-path", true) },
                Operation = delegate(string[] args){ AddToPackage(args[0], args[1], args.Length>2 ? args[2] : args[1]); }
            },
            new Command
            {
                Name = "--list", Aliases = new[]{"-l"},
                HelpText = "Lists all files contained withing given .PAK package.",
                Arguments = new[]{ new Argument("pak-path") },
                Operation = delegate(string[] args){ ListPackageContent(args[0]); }
            },
            new Command
            {
                Name = "--convert-from-xnb", Aliases = new[]{"-x"},
                HelpText = "Attempts to convert given XNB input (this can be a path to a single asset or an entire directory) " +
                "and save it at given output (if input is a directory, dumps all converted files in specified path). " +
                "Directories are treated recursively.",
                Arguments = new[]{ new Argument("xnb-input"), new Argument("file-output") },
                Operation = delegate(string[] args){ ConvertFromXNB(args[0], args[1]); }
            },
            new Command
            {
                Name = "--convert-to-xnb", Aliases = new[]{"-X"},
                HelpText = "Attempts to convert given input (this can be a path to a single file or an entire directory) " +
                "into XNB file(s) and save it at given output (if input is a directory, dumps all converted files in specified path). " +
                "Directories are treated recursively.",
                Arguments = new[]{ new Argument("file-input"), new Argument("xnb-output") },
                Operation = delegate(string[] args){ ConvertIntoXNB(args[0], args[1]); }
            },
        };


        private static bool FindCommand(string name, out Command outCommand)
        {
            var validCommands = Commands.Where(command => command.Name == name || command.Aliases.Contains(name));
            if (validCommands.Any())
            {
                outCommand = validCommands.First();
                return true;
            }
            else
            {
                outCommand = new();
                return false;
            }
        }

        public static void ParseCommandLine(string[] args)
        {
            if (args.Length == 1 && args[0] == "debug")
            {
                Console.WriteLine("Running debug mode! Type your command line arguments: ");
                var input = Console.ReadLine() ?? "";
                args = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m => m.Value).ToArray();
            }

            if (args.Length == 0) return;

            if (!FindCommand(args[0], out Command command))
            {
                Console.WriteLine($"Unknown command \"{args[0]}\".");
                Console.WriteLine($"Type \"FEZRepacker.exe --help\" for a list of commands.");
                return;
            }

            string[] cmdArgs = args.Skip(1).ToArray();

            int maxArgs = command.Arguments.Length;
            int minArgs = command.Arguments.Count(arg => !arg.Optional);

            if (cmdArgs.Length < minArgs || cmdArgs.Length > maxArgs)
            {
                Console.WriteLine($"Invalid usage for command \"{args[0]}\" (incorrect number of parameters).");
                Console.WriteLine($"Use \"FEZRepacker.exe --help {args[0]}\" for a usage instruction for that command.");
                return;
            }

            try
            {
                command.Operation!(cmdArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while executing command: {ex.Message}");
            }
        }

    }
}
