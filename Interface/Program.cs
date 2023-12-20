using System.Globalization;

using static FEZRepacker.Interface.CommandLine;

namespace FEZRepacker.Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // showoff
            Console.WriteLine(
                "================================================\n" +
                "= FEZRepacker 0.4 by Krzyhau & FEZModding Team =\n" +
                "================================================\n"
            );

            // keep number decimals consistent
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            if (args.Length > 0)
            {
                CommandLine.ParseCommandLine(args);
            }
            else
            {
                // run user interface
                Console.Write('\a');//alert user in case the program started in the background

                //inject special commands into the command list if running without arguments
                {
                    Command clearcommand = new Command
                    {
                        Name = "clear",
                        Aliases = new[] { "cls" },
                        HelpText = "Clears the console output",
                        Arguments = Array.Empty<Argument>(),
                        Operation = (s) => Console.Clear()
                    };
                    Command exitcommand = new Command
                    {
                        Name = "exit",
                        Aliases = new[] { "end", "stop", "terminate" },
                        HelpText = "Exits the program",
                        Arguments = Array.Empty<Argument>(),
                        Operation = (s) => { Console.WriteLine("Exiting..."); Environment.Exit(0); }
                    };
                    CommandLine.Commands = CommandLine.Commands.Append(clearcommand).Append(exitcommand).ToArray();
                }
                bool writehowgethelp = true;
                while (true)
                {
                    if(writehowgethelp)
                    {
                        Console.WriteLine("In order to get usage help, use '--help' argument.");
                    }
                    //Console.WriteLine("Arguments: " + string.Join(", ", args));
                    Console.WriteLine("To exit, input exit");

                    Console.WriteLine();
                    Console.Write("> FEZRepacker.exe ");
                    string? line = Console.ReadLine();
                    if (line == null)
                    {
                        Console.WriteLine("\nEncountered a null input. Exiting...");
                        return;//this shouldn't happen unless the user presses the "break" button, or if the input was read from a file or something
                    }
                    else
                    {
                        args = ParseArguments(line);
                        if (args.Length > 0)
                        {
                            switch (args[0].ToLower())//Note: probably don't need this switch statement since these commands are appended to CommandLine.Commands 
                            {
                                case "cls":
                                case "clear":
                                    Console.Clear();
                                    break;
                                case "exit":
                                case "end":
                                case "stop":
                                case "terminate":
                                    Console.WriteLine("Exiting...");
                                    return;
                                    //break;
                                default:
                                    writehowgethelp = !CommandLine.ParseCommandLine(args);
                                    Console.WriteLine();
                                    break;
                            }
                        }
                        else
                        {
                            writehowgethelp = true;
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Converts the argument string into an array of strings representing the arguments.
        /// </summary>
        /// <param name="argstr">The string to convert</param>
        /// <returns>The array of arguments</returns>
        private static string[] ParseArguments(string argstr)
        {
            //basically, it splits the string at spaces that are not in quotation marks, and strips quotation marks

            //basically does the PCRE '/(?|"([^"]+?)"|([^ "]+))/' but regexes are slow and wouldn't allow for easy escaping of double quotes
            //Note: if you change the [^"]+? in the above regex to [^"]*? it could capture empty strings in double quotes

            //Note: in this code, '\n' functions as the argument delimiter. 
            char[] parmChars = argstr.ToCharArray();
            bool inQuote = false;
            for (int index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"')
                {
                    inQuote = !inQuote;
                    parmChars[index] = '\n';//strip quotes
                }
                if (!inQuote && parmChars[index] == ' ')
                {
                    parmChars[index] = '\n';//new argument
                }
            }
            return (new string(parmChars)).Split('\n')
                    .Where(s => !string.IsNullOrEmpty(s))//This `Where` statement removes blank arguments
                    .ToArray();
        }
    }
}
