using System.IO;

using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.XNB;

namespace FEZRepacker.Interface
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
                var actualOutputDir = outputDir;
                if (packagePath.EndsWith("Music.pak"))
                {
                    actualOutputDir = Path.Combine(outputDir, "music");
                }
                UnpackPackage(packagePath, actualOutputDir, unpackingMode);
            }
        }

        public static void UnpackPackage(string pakPath, string outputDir, UnpackingMode mode)
        {
            if (Path.GetExtension(pakPath) != ".pak")
            {
                throw new Exception("A path must lead to a .PAK file.");
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            using var pakStream = File.OpenRead(pakPath);
            using var pakReader = new PakReader(pakStream);

            Console.WriteLine($"Unpacking archive {pakPath} containing {pakReader.FileCount} files...");

            int filesDone = 0;
            foreach (var pakFile in pakReader.ReadFiles())
            {
                var extension = pakFile.DetectedFileExtension;

                Console.WriteLine(
                    $"({filesDone + 1}/{pakReader.FileCount})" +
                    $"{pakFile.Path} ({(extension.Length == 0 ? "unknown" : extension)} file," +
                    $"size: {pakFile.Size} bytes)"
                );

                try
                {
                    var outputBundle = FileBundle.Single(pakFile.Data, ".xnb");

                    if (mode == UnpackingMode.DecompressedXNB)
                    {
                        outputBundle = FileBundle.Single(XnbCompressor.Decompress(pakFile.Data), ".xnb");
                    }
                    else if (mode == UnpackingMode.Converted)
                    {
                        var converter = new XnbConverter();
                        outputBundle = converter.Convert(pakFile.Data);
                        var formatName = converter.HeaderValid ? converter.FileType.Name.Replace("Reader", "") : "";
                        if (converter.Converted)
                        {
                            extension = converter.FormatConverter!.FileFormat;
                            var storageTypeName = (outputBundle.Count > 1 ? "bundle" : "file");
                            Console.WriteLine($"  Format {formatName} converted into {extension} {storageTypeName}.");
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

                    outputBundle.BundlePath = Path.Combine(outputDir, pakFile.Path + extension);
                    var outputDirectory = Path.GetDirectoryName(outputBundle.BundlePath) ?? "";
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    foreach(var outputFile in outputBundle)
                    {
                        using var fileOutputStream = File.Open(outputBundle.BundlePath + outputFile.Extension, FileMode.Create);
                        outputFile.Data.CopyTo(fileOutputStream);
                    }

                    outputBundle.Dispose();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to unpack {pakFile.Path} - {ex.Message}");
                }
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
            using var pakReader = new PakReader(pakStream);

            Console.WriteLine($"PAK package \"{pakPath}\" with {pakReader.FileCount} files.");
            Console.WriteLine();

            foreach (var item in pakReader.ReadFiles())
            {
                var typeText = item.DetectedFileExtension.Length == 0 ? "unknown" : item.DetectedFileExtension;
                Console.WriteLine($"{item.Path} ({typeText} file, size: {item.Size} bytes)");
            }
        }

        public static void ConvertFromXNB(string inputPath, string outputPath)
        {
            var xnbFilesToConvert = new List<string>();

            if (Directory.Exists(inputPath))
            {
                xnbFilesToConvert = Directory.GetFiles(inputPath, "*.xnb", SearchOption.AllDirectories).ToList();
                Console.WriteLine($"Found {xnbFilesToConvert.Count()} XNB files in given directory.");
            }
            else if (File.Exists(inputPath))
            {
                xnbFilesToConvert.Add(inputPath);
                if (Path.GetExtension(inputPath) != ".xnb")
                {
                    throw new Exception("An input file must be an .XNB file.");
                }
                inputPath = Path.GetDirectoryName(inputPath) ?? "";
            }
            else
            {
                throw new FileNotFoundException("Specified input path does not lead to any file or a directory");
            }

            if (outputPath.Length == 0)
            {
                outputPath = inputPath;
            }
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var filesDone = 0;
            foreach (var xnbPath in xnbFilesToConvert)
            {
                Console.WriteLine($"({filesDone + 1}/{xnbFilesToConvert.Count}) {xnbPath}");

                using var xnbStream = File.OpenRead(xnbPath);

                var converter = new XnbConverter();
                var outputBundle = converter.Convert(xnbStream);
                var formatName = converter.HeaderValid ? converter.FileType.Name.Replace("Reader", "") : "";
                if (converter.Converted)
                {
                    outputBundle.MainExtension = converter.FormatConverter!.FileFormat;
                    var storageTypeName = (outputBundle.Count > 1 ? "bundle" : "file");
                    Console.WriteLine($"  Format {formatName} converted into {outputBundle.MainExtension} {storageTypeName}.");
                }
                else
                {
                    Console.WriteLine(converter.HeaderValid ? $"  Unknown format {formatName}." : $"  Not a valid XNB file." + " Skipping.");
                    continue;
                }

                var relativePath = Path.GetRelativePath(inputPath, xnbPath)
                    .Replace("/", "\\")
                    .Replace(".xnb", "", StringComparison.InvariantCultureIgnoreCase);
                outputBundle.BundlePath = Path.Combine(outputPath, relativePath + outputBundle.MainExtension);
                var outputDirectory = Path.GetDirectoryName(outputBundle.BundlePath) ?? "";
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                foreach (var outputFile in outputBundle)
                {
                    using var fileOutputStream = File.Open(outputBundle.BundlePath + outputFile.Extension, FileMode.Create);
                    outputFile.Data.CopyTo(fileOutputStream);
                }

                filesDone++;
            }
        }

        public static void ConvertIntoXNB(string inputPath, string outputPath)
        {
            var fileNames = new string[0];

            if (Directory.Exists(inputPath))
            {
                fileNames = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories);
                Console.WriteLine($"Found {fileNames.Length} files.");
            }
            else if (File.Exists(inputPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(inputPath);
                inputPath = Path.GetDirectoryName(inputPath) ?? "";
                fileNames = Directory.GetFiles(inputPath, $"{fileName}.*");
                Console.WriteLine($"Found {fileNames.Length} linked files for a file bundle.");
            }
            else
            {
                throw new FileNotFoundException("Specified input path does not lead to any file or a directory");
            }

            if (outputPath.Length == 0)
            {
                outputPath = inputPath;
            }
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // load and transform file list into bundles
            var fileList = new Dictionary<string, Stream>();
            foreach (var filePath in fileNames)
            {
                var relativePath = Path.GetRelativePath(inputPath, filePath).Replace("/", "\\").ToLower();
                fileList[relativePath] = File.OpenRead(filePath);
            }
            var fileBundles = FileBundle.BundleFiles(fileList);
            Console.WriteLine($"Converting {fileBundles.Count()} file bundles.");


            var xnbAssets = new Dictionary<(string Path, string Extension), Stream>();

            // convert
            Console.WriteLine($"Converting {fileBundles.Count()} assets...");
            var filesDone = 0;
            foreach (var fileBundle in fileBundles)
            {
                Console.WriteLine($"({filesDone + 1}/{fileBundles.Count}) {fileBundle.BundlePath}");

                try
                {
                    var deconverter = new XnbDeconverter();

                    var deconverterStream = deconverter.Deconvert(fileBundle);

                    if (deconverter.Converted)
                    {
                        Console.WriteLine($"  Format {fileBundle.MainExtension} deconverted into {deconverter.FormatConverter!.FormatName} XNB asset.");
                        xnbAssets.Add((fileBundle.BundlePath, ".xnb"), deconverterStream);
                    }
                    else
                    {
                        Console.WriteLine($"  Format {fileBundle.MainExtension} doesn't have a converter - packing asset as raw files.");

                        foreach (var file in fileBundle)
                        {
                            file.Data.Seek(0, SeekOrigin.Begin);
                            var ext = fileBundle.MainExtension + file.Extension;
                            xnbAssets.Add((fileBundle.BundlePath, ext), file.Data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to convert asset {fileBundle.BundlePath} - {ex.Message}");
                }
                filesDone++;
            }

            Console.WriteLine($"Saving {xnbAssets.Count()} XNB assets...");

            foreach (var assetRecord in xnbAssets)
            {
                var assetPath = assetRecord.Key.Path;
                var assetExtension = assetRecord.Key.Extension;
                var asset = assetRecord.Value;

                var assetOutputFullPath = Path.Combine(outputPath, $"{assetPath}{assetExtension}");

                var directoryPath = Path.GetDirectoryName(assetOutputFullPath) ?? "";
                if (directoryPath.Length > 0 && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using var assetFile = File.Create(assetOutputFullPath);
                asset.CopyTo(assetFile);
            }
        }

        public static void AddToPackage(string inputPath, string outputPackagePath, string includePackagePath)
        {
            // prepare include package
            bool shouldUseIncludePackage = false;

            if (File.Exists(includePackagePath))
            {
                if (Path.GetExtension(includePackagePath) != ".pak")
                {
                    throw new Exception("Included package path must lead to a .PAK file.");
                }
                shouldUseIncludePackage = true;
                Console.WriteLine($"Using {includePackagePath} as a base for package creation.");
            }

            // get a list of files to load
            if (!Directory.Exists(inputPath))
            {
                throw new Exception("Given input directory does not exist.");
            }

            string[] fileNamesToAdd = Directory.GetFiles(inputPath, "*.*", SearchOption.AllDirectories);
            Console.WriteLine($"Found {fileNamesToAdd.Length} files.");

            // load and transform file list into bundles
            var fileList = new Dictionary<string, Stream>();
            foreach (var filePath in fileNamesToAdd)
            {
                var relativePath = Path.GetRelativePath(inputPath, filePath).Replace("/", "\\").ToLower();
                fileList[relativePath] = File.OpenRead(filePath);
            }
            var fileBundlesToAdd = FileBundle.BundleFiles(fileList);

            // create a temporary pak file
            // we're doing this because output package path might be the same as include package path
            // and since we want to write to one while reading to the other one, temp file is needed.
            string tempPakName = Path.GetTempPath() + "repacker_pak_" + Guid.NewGuid().ToString() + ".pak";
            using var tempPakStream = File.Open(tempPakName, FileMode.Create);
            using var tempPak = new PakWriter(tempPakStream);

            // add files from include package
            if (shouldUseIncludePackage)
            {
                using var includePackageStream = File.Open(includePackagePath, FileMode.Open);
                using var includePackage = new PakReader(includePackageStream);
                foreach (var file in includePackage.ReadFiles())
                {
                    if (fileList.ContainsKey(file.Path))
                    {
                        Console.WriteLine($"File {file.Path} exists in include package already and it's going to be replaced.");
                        continue;
                    }
                    tempPak.WriteFile(file.Path, file.Data);
                }
            }

            // convert assets and add them to temp pak
            Console.WriteLine($"Converting {fileBundlesToAdd.Count()} assets...");
            var filesDone = 0;
            foreach (var fileBundle in fileBundlesToAdd)
            {
                Console.WriteLine($"({filesDone + 1}/{fileBundlesToAdd.Count}) {fileBundle.BundlePath}");

                try
                {
                    var deconverter = new XnbDeconverter();

                    using var deconverterStream = deconverter.Deconvert(fileBundle);

                    if (deconverter.Converted)
                    {
                        Console.WriteLine($"  Format {fileBundle.MainExtension} deconverted into {deconverter.FormatConverter!.FormatName} XNB asset.");
                        tempPak.WriteFile(fileBundle.BundlePath, deconverterStream);
                    }
                    else
                    {
                        Console.WriteLine($"  Format {fileBundle.MainExtension} doesn't have a converter - packing asset as raw files.");

                        foreach(var file in fileBundle)
                        {
                            file.Data.Seek(0, SeekOrigin.Begin);
                            tempPak.WriteFile(fileBundle.BundlePath, file.Data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to convert asset {fileBundle.BundlePath} - {ex.Message}");
                }
                filesDone++;

                // we're done with that file bundle - get rid of it
                fileBundle.Dispose();
            }

            // finalize - move temp file to output package path
            Console.WriteLine($"Packed {tempPak.FileCount} assets into {outputPackagePath}...");

            tempPak.Dispose();
            tempPakStream.Close();

            if (File.Exists(outputPackagePath))
            {
                File.Delete(outputPackagePath);
            }
            File.Move(tempPakName, outputPackagePath);
            
        }
    }
}
