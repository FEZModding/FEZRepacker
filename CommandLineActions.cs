using FEZRepacker.Converter.PAK;
using FEZRepacker.Converter.XNB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker
{
    internal static partial class CommandLine
    {
        public enum UnpackingMode
        {
            Raw,
            DecompressedXNB,
            Converted
        }

        private static void ShowHelpFor(Command command)
        {
            string allNames = command.Name;
            if (command.Aliases.Length > 0)
            {
                allNames = $"[{command.Name}, {String.Join(", ", command.Aliases)}]";
            }

            string argsStr = String.Join(" ", command.Arguments.Select(arg => arg.Optional ? $"<{arg.Name}>" : $"[{arg.Name}]"));

            Console.WriteLine($"Usage: FEZRepacker.exe {allNames} {argsStr}");
            Console.WriteLine($"Description: {command.HelpText}");
        }

        private static void CommandHelp(string[] args)
        {
            if (args.Length > 0)
            {
                if (!FindCommand(args[0], out Command command))
                {
                    Console.WriteLine($"Unknown command \"{args[0]}\".");
                    Console.WriteLine($"Type \"FEZRepacker.exe --help\" for a list of commands.");
                    return;
                }
                ShowHelpFor(command);
                return;
            }

            foreach (var command in Commands)
            {
                ShowHelpFor(command);
                Console.WriteLine();
            }
        }

        public static void UnpackGameAssets(string contentPath, string outputDir, UnpackingMode unpackingMode)
        {
            var packagePaths = new string[] { "Essentials.pak", "Music.pak", "Other.pak", "Updates.pak" }
                .Select(path => Path.Combine(contentPath, path)).ToArray();

            foreach (var packagePath in packagePaths)
            {
                if (!File.Exists(packagePath))
                {
                    throw new Exception($"Given directory is not FEZ's Content directory (missing {Path.GetFileName(packagePath)}).");
                }
            }

            foreach (var packagePath in packagePaths)
            {
                UnpackPackage(packagePath, outputDir, unpackingMode);
            }
        }

        public static void UnpackPackage(string pakPath, string outputDir, UnpackingMode mode)
        {
            if(Path.GetExtension(pakPath) != ".pak")
            {
                throw new Exception("A path must lead to a .PAK file.");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            using var pakStream = File.OpenRead(pakPath);
            var pak = PakContainer.Read(pakStream);

            Console.WriteLine($"Unpacking archive {pakPath} containing {pak.Count} files...");

            int filesDone = 0;
            foreach(var pakFile in pak)
            {
                var extension = pakFile.GetExtensionFromHeaderOrDefault();

                Console.WriteLine($"({filesDone+1}/{pak.Count}) {pakFile.Path} ({extension} file, size: {pakFile.Size} bytes)");

                using var fileStream = pakFile.Open();

                var outputStream = fileStream;

                if(mode == UnpackingMode.DecompressedXNB)
                {
                    outputStream = XnbCompressor.Decompress(fileStream);
                }
                else if(mode == UnpackingMode.Converted)
                {
                    var converter = new XnbConverter();
                    outputStream = converter.Convert(fileStream);
                    var formatName = converter.HeaderValid ? converter.FileType.Name.Replace("Reader", "") : "";
                    if (converter.Converted)
                    {
                        extension = converter.FormatConverter!.FileFormat;
                        Console.WriteLine($"  Format {formatName} converted into {extension} file.");
                    }
                    else
                    {
                        if (converter.HeaderValid)
                        {
                            Console.WriteLine($"  Unknown format {formatName} - saving as XNB asset.");
                        }
                        else
                        {
                            Console.WriteLine($"  Not a valid XNB file - saving with detected extension ({extension}).");
                        }
                        
                    }
                }

                var outputFileName = Path.Combine(outputDir, pakFile.Path + extension);
                var outputDirectory = Path.GetDirectoryName(outputFileName) ?? "";
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                using var fileOutputStream = File.Open(outputFileName, FileMode.Create);
                outputStream.CopyTo(fileOutputStream);

                filesDone++;
            }
        }

        public static void ListPackageContent(string pakPath)
        {
            if (Path.GetExtension(pakPath) != ".pak")
            {
                throw new Exception("A path must lead to a .PAK file.");
            }

            using var pakStream = File.OpenRead(pakPath);
            var pak = PakContainer.Read(pakStream);

            Console.WriteLine($"PAK package \"{pakPath}\" with {pak.Count} files.");
            Console.WriteLine();

            foreach(var item in pak)
            {
                var extension = item.GetExtensionFromHeaderOrDefault("unknown");
                Console.WriteLine($"{item.Path} ({extension} file, size: {item.Size} bytes)");
            }
        }

        public static void ConvertFromXNB(string inputPath, string outputPath)
        {
            Console.WriteLine("Feature currently not supported.");
        }

        public static void ConvertIntoXNB(string inputPath, string outputPath)
        {
            Console.WriteLine("Feature currently not supported.");
        }

        public static void AddToPackage(string inputPath, string outputPackagePath, string includePackagePath)
        {
            // prepare package
            PakContainer pak = new PakContainer();

            if (File.Exists(includePackagePath))
            {
                if (Path.GetExtension(includePackagePath) != ".pak")
                {
                    throw new Exception("Included package path must lead to a .PAK file.");
                }
                using var includeStream = File.OpenRead(includePackagePath);
                pak = PakContainer.Read(includeStream);
                Console.WriteLine($"Using {includePackagePath} as a base for package creation.");
            }

            // get a list of files to load
            if (!Directory.Exists(inputPath))
            {
                throw new Exception("Given input directory does not exist.");
            }

            string[] filesToAdd = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories);
            Console.WriteLine($"Packing {filesToAdd.Length} files into {outputPackagePath}...");

            // load, convert and add files to the package
            var filesDone = 0;
            foreach(var filePath in filesToAdd)
            {
                Console.WriteLine($"({filesDone + 1}/{filesToAdd.Length}) {filePath}");

                var extension = Path.GetExtension(filePath).ToLower();
                var newExtension = extension;
                using var fileStream = File.OpenRead(filePath);

                var deconverter = new XnbDeconverter(extension);

                using var deconverterStream = deconverter.Deconvert(fileStream);

                if (deconverter.Converted)
                {
                    Console.WriteLine($"  Format {extension} deconverted into {deconverter.FormatConverter!.FormatName} XNB asset.");
                    newExtension = ".xnb";
                }
                else
                {
                    if(extension.Equals(".xnb", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"  File is an XNB asset already - packing directly.");
                    }
                    else
                    {
                        Console.WriteLine($"  Format {extension} doesn't have a converter - packing as a raw file.");
                    }
                }

                var pakFilePath = Path.GetRelativePath(inputPath, filePath).Replace("/", "\\").ToLower();
                pakFilePath = pakFilePath.Substring(0, pakFilePath.Length - extension.Length);

                int removed = pak.RemoveAll(file => file.Path == pakFilePath && file.GetExtensionFromHeaderOrDefault() == newExtension);
                if(removed > 0)
                {
                    Console.WriteLine($"  This file replaces {removed} file{(removed > 1 ? "s" : "")} that has been in the package already.");
                }

                pak.Add(PakFile.Read(pakFilePath, deconverterStream));

                filesDone++;
            }

            // save package
            using var fileOutputStream = File.Open(outputPackagePath, FileMode.Create);
            pak.Save(fileOutputStream);
        }
    }
}
