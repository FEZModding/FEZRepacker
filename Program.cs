using System.Globalization;

namespace FEZRepacker
{
    class Program
    {
        static Dictionary<string, Action<string[]>> Commands = new Dictionary<string, Action<string[]>>()
        {
            {"?", CommandHelp },
            {"help", CommandHelp },
            {"/?", CommandHelp },
            {"/help", CommandHelp },

            {"unpack", CommandUnpack},
            {"pack", CommandPack},
            {"add", CommandAdd },
        };

        // unpacks PAK file located in given path into given directory location
        static void CommandUnpack(string[] args)
        {
            if(args.Length != 2)
            {
                throw new ArgumentException("Invalid number of parameters.");
            }

            string pakPath = args[0];
            string unpackPath = args[1];

            PAKContainer pak = PAKContainer.LoadPak(pakPath);
            if (pak == null)
            {
                throw new FileLoadException("Couldn't load PAK file.");
            }

            Console.WriteLine($"Loaded .pak package containing {pak.FileCount} files.\n");

            foreach ((var name, var file) in pak.Files)
            {
                Console.WriteLine($"\"{name}\" ({file.GetInfo()})");
            }

            Console.WriteLine($"\nAttempting to save files to {unpackPath}.\n");

            pak.SaveContent(unpackPath);
        }

        // packs directory in given location into a pak file with given name
        static void CommandPack(string[] args)
        {

        }


        // packs files in given location and adds them into existing archive
        static void CommandAdd(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                throw new ArgumentException("Invalid number of parameters.");
            }

            string pakPath = args[0];
            string assetsPath = args[1];
            string newPakPath = args.Length > 2 ? args[2] : pakPath;

            PAKContainer pak = PAKContainer.LoadPak(pakPath);
            if (pak == null)
            {
                throw new FileLoadException("Couldn't load PAK file.");
            }
            Console.WriteLine($"Loaded .pak package containing {pak.FileCount} files.");

            Console.WriteLine($"Converting assets from {assetsPath} into PAK archive...");
            pak.LoadContent(assetsPath);

            Console.WriteLine("Saving PAK file...");
            pak.SavePak(newPakPath);
        }

        static void CommandHelp(string[] args)
        {
            Console.WriteLine(
                "To unpack files from FEZ's .pak file into specific directory, use:\n\n" +
                "FEZRepacker.exe unpack <source> <destination>\n\n" +
                "  source       Specifies the FEZ's .pak file to be unpacked.\n" +
                "  destination  Specifies the directory where unpacked files will be saved.\n\n" +

                "To pack files as FEZ's .pak file, use:\n\n" +
                "FEZPacker.exe pack <source> <destination>\n\n" +
                "  source       Specifies the directory where files to packed are located.\n" +
                "  destination  Specifies the directory and filename for the packed file.\n\n" +

                "To add files into FEZ's .pak file, use:\n\n" +
                "FEZPacker.exe add <target> <source> [destination]\n\n" +
                "  target       Specifies the FEZ's .pak file into which files will be added.\n" +
                "  source       Specifies the directory where files to packed are located.\n" +
                "  destination  Specifies the directory and filename for the new packed file. If not set, uses the target name (overrides it).\n\n"
            );
        }

        
        static void Main(string[] args)
        {
            // keep number decimals consistent
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            // showoff
            Console.WriteLine(
                "==============================\n" +
                "= FEZRepacker 0.1 by Krzyhau =\n" +
                "==============================\n\n"
            );

            // handle commands
            bool validCommand = false;

            if (args.Length == 0)
            {
                Console.WriteLine("FEZPacker made by Krzyhau. Used to pack and unpack .pak files used in FEZ 1.12.\n");
            }
            else if (!Commands.ContainsKey(args[0]))
            {
                Console.WriteLine("Invalid command!");
            }
            else
            {
                string command = args[0].ToLower();
                string[] arguments = args.Skip(1).ToArray();
                try
                {
                    Commands[command](arguments);
                    validCommand = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error while trying to execute command \"{command}\": {ex.Message}");
                }
            }

            if (!validCommand)
            {
                Console.WriteLine("Use \"FEZPacker.exe help\" for more help.");
            }

        }
    }
}