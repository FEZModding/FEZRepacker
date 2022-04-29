using System;

namespace FEZRepacker
{
    internal class Program
    {
        static bool VerifyArgs(string[] args, out string error)
        {
            error = "";

            if (args.Length > 0 && (args[0] == "/?" || args[0] == "help"))
            {
                error = "FEZPacker made by Krzyhau. Used to pack and unpack .pak files used in FEZ 1.12.";
                return false;
            }
            else if (args.Length != 3)
            {
                error = "Incorrect number of parameters.";
                return false;
            }
            if (args[0] != "unpack" && args[0] != "pack")
            {
                error = "Incorrect parameter.";
                return false;
            }

            return true;
        }

        // unpacks PAK file located in given path into given directory location
        static void CommandUnpack(string pakPath, string unpackPath)
        {
            PAKContainer pak = PAKContainer.LoadPak(pakPath);
            if (pak == null)
            {
                Console.WriteLine("Failed to load PAK file.");
                return;
            }

            Console.WriteLine($"Loaded .pak package containing {pak.FileCount} files.\n");

            foreach ((var name, var file) in pak.Files)
            {
                Console.WriteLine($"\"{name}\" ({file.GetInfo()})");

                if(file is XNBFile)
                {
                    ((XNBFile)file).ReadXNBContent();
                }
            }

            Console.WriteLine($"\nAll main types of content:");

            foreach (var reader in XNBContent.MainReaders)
            {
                Console.WriteLine($" - {reader}");
            }

            Console.WriteLine("$\nAttempting to save zuish font file");
            if (pak.Files.ContainsKey("fonts\\zuish"))
            {
                if (!Directory.Exists(unpackPath)) Directory.CreateDirectory(unpackPath);
                File.WriteAllBytes($"{unpackPath}/zuish.xnb", pak.Files["fonts\\zuish"].Content);
            }

            //if (!Directory.Exists(unpackPath)) Directory.CreateDirectory(unpackPath);
            //File.WriteAllBytes($"{unpackPath}/test.bin", testFile.Content);
        }

        // packs directory in given location into a pak file with given name
        static void CommandPack(string unpackPath, string pakPath)
        {
            
        }

        static void Main(string[] args)
        {
            // showoff
            Console.WriteLine(
                "============================\n" +
                "= FEZRepacker 0.1 by Krzyhau =\n" +
                "============================\n\n"
            );

            // printing out help string if used program incorrectly
            if (!VerifyArgs(args, out string error))
            {
                Console.WriteLine($"{error}\n\n");
                Console.WriteLine(
                    "To unpack files from FEZ's .pak file into specific directory, use:\n\n" +
                    "FEZRepacker.exe unpack [source] [destination]\n\n" +
                    "  source       Specifies the FEZ's .pak file to be unpacked.\n" +
                    "  destination  Specifies the directory where unpacked files will be saved.\n\n" +

                    "To pack files into FEZ's .pak file, use:\n\n" +
                    "FEZPacker.exe pack [source] [destination]\n\n" +
                    "  source       Specifies the directory where files to packed are located.\n" +
                    "  destination  Specifies the directory and filename for the packed file.\n\n"
                );
            }

            // execute proper command
            if (args[0] == "pack")
            {
                CommandPack(args[1], args[2]);
            } 
            else if (args[0] == "unpack")
            {
                CommandUnpack(args[1], args[2]);
            }

        }
    }
}