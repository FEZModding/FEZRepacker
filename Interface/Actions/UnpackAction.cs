using System.CommandLine;

using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

using static FEZRepacker.Interface.CommandLineOptions;
using static FEZRepacker.Interface.CommandLineUtils;

namespace FEZRepacker.Interface.Actions
{
    internal class UnpackAction : ICommandLineAction
    {
        public enum UnpackingMode
        {
            [Aliases("c")] Converted, // Default value
            [Aliases("d")] DecompressedXnb,
            [Aliases("r")] Raw
        }

        public string Name => "--unpack";

        public string[] Aliases => ["-u", UnpackDecompressedAlias, UnpackRawAlias];

        public string Description =>
            "Unpacks entire .PAK package into specified directory and attempts to process XNB assets.\n\n"
            + $"The '{UnpackDecompressedAlias}' and '{UnpackRawAlias}' aliases are provided for backwards compatibility\n"
            + $"and they reproduce the command behaviour with the '--mode' flags "
            + $"'{ArgumentsOf(UnpackingMode.DecompressedXnb)}' "
            + $"and '{ArgumentsOf(UnpackingMode.Raw)}' respectively.";

        public Argument[] Arguments => [_pakFile, _destinationDirectory];

        public Option[] Options => [_unpackingMode, UseArtObjectLegacyBundles, UseTrileSetLegacyBundles];

        private readonly Argument<FileInfo> _pakFile = new("pak-file")
        {
            Description = "Source path of the .PAK package"
        };

        private readonly Argument<DirectoryInfo> _destinationDirectory = new("destination-directory")
        {
            Description = "Target path of the destination directory (creates one if doesn't exist)"
        };

        private readonly Option<UnpackingMode> _unpackingMode = new("--mode", "-m")
        {
            Description = "Unpacking mode\n"
                          + $"  <{ArgumentsOf(UnpackingMode.Converted)}> (default): Convert XNB assets into their corresponding format\n"
                          + $"  <{ArgumentsOf(UnpackingMode.DecompressedXnb)}>': Decompress XNB assets, but do not convert them\n"
                          + $"  <{ArgumentsOf(UnpackingMode.Raw)}>': Leave XNB assets in their original form\n",
            CustomParser = CustomAliasedEnumParser<UnpackingMode>
        };

        private const string UnpackDecompressedAlias = "--unpack-decompressed";
        
        private const string UnpackRawAlias = "--unpack-raw";

        public void Execute(ParseResult result)
        {
            var pakFile = result.GetRequiredValue(_pakFile);
            var destinationDirectory = result.GetRequiredValue(_destinationDirectory);
            
            UnpackingMode unpackingMode;
            switch (result.CommandResult.IdentifierToken.Value)
            {
                // For backward compatibility, the value of the "--mode" option
                // will be overwritten when using these older commands.
                case UnpackDecompressedAlias:
                    Console.WriteLine($"Warning! The '{_unpackingMode.Name}' option will be ignored.");
                    unpackingMode = UnpackingMode.DecompressedXnb;
                    break;
                case UnpackRawAlias:
                    Console.WriteLine($"Warning! The '{_unpackingMode.Name}' option will be ignored.");
                    unpackingMode = UnpackingMode.Raw;
                    break;
                default:
                    unpackingMode = result.GetValue(_unpackingMode);
                    break;
            }
            
            var settings = new FormatConverterSettings
            {
                UseLegacyArtObjectBundle = result.GetValue(UseArtObjectLegacyBundles),
                UseLegacyTrileSetBundle = result.GetValue(UseTrileSetLegacyBundles)
            };
            UnpackPackage(pakFile, destinationDirectory, unpackingMode, settings);
        }

        public static FileBundle UnpackFile(string extension, Stream data, UnpackingMode mode, FormatConverterSettings settings)
        {
            if (extension != ".xnb")
            {
                return FileBundle.Single(data, extension);
            }

            switch (mode)
            {
                case UnpackingMode.Raw:
                    return FileBundle.Single(data, extension);

                case UnpackingMode.DecompressedXnb:
                    return FileBundle.Single(XnbCompressor.Decompress(data), ".xnb");

                case UnpackingMode.Converted:
                    var initialStreamPosition = data.Position;
                    FileBundle outputBundle;
                    try
                    {
                        var outputData = XnbSerializer.Deserialize(data)!;
                        outputBundle = FormatConversion.Convert(outputData, settings);
                        Console.WriteLine(
                            $"  {outputData.GetType().Name} converted into {outputBundle.MainExtension} format.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Cannot deserialize XNB file: {ex.Message}. Saving raw file instead.");
                        data.Seek(initialStreamPosition, SeekOrigin.Begin);
                        return FileBundle.Single(data, extension);
                    }
                    return outputBundle;

                default:
                    return new FileBundle();
            }
        }

        public static void UnpackPackage(FileInfo pakFile, DirectoryInfo outputDir, UnpackingMode mode, FormatConverterSettings settings)
        {
            if (pakFile.Extension != ".pak")
            {
                throw new Exception("A path must lead to a .PAK file.");
            }

            if (!outputDir.Exists)
            {
                outputDir.Create();
            }

            using var pakStream = File.OpenRead(pakFile.FullName);
            using var pakReader = new PakReader(pakStream);

            Console.WriteLine($"Unpacking archive {pakFile} containing {pakReader.FileCount} files...");

            int filesDone = 0;
            foreach (var fileRecord in pakReader.ReadFiles())
            {
                var extension = fileRecord.FindExtension();

                Console.WriteLine(
                    $"({filesDone + 1}/{pakReader.FileCount})" +
                    $"{fileRecord.Path} ({(extension.Length == 0 ? "unknown" : extension)} file," +
                    $"size: {fileRecord.Length} bytes)"
                );

                try
                {
                    using var fileStream = fileRecord.Open();
                    using var outputBundle = UnpackFile(extension, fileStream, mode, settings);

                    outputBundle.BundlePath = Path.Combine(outputDir.FullName, fileRecord.Path);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputBundle.BundlePath) ?? "");

                    foreach (var outputFile in outputBundle.Files)
                    {
                        var fileName = outputBundle.BundlePath + outputBundle.MainExtension + outputFile.Extension;
                        using var fileOutputStream = File.Open(fileName, FileMode.Create);
                        outputFile.Data.CopyTo(fileOutputStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to unpack {fileRecord.Path} - {ex.Message}");
                }

                filesDone++;
            }
        }
    }
}