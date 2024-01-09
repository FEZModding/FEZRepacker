using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.XNB;

namespace FEZRepacker.Interface.Actions
{
    internal abstract class UnpackAction : CommandLineAction
    {
        public enum UnpackingMode
        {
            Raw,
            DecompressedXNB,
            Converted
        }
        protected abstract UnpackingMode Mode { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string[] Aliases { get; }

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument("pak-path"),
            new CommandLineArgument("destination-folder")
        };

        public void Execute(string[] args)
        {
            var pakPath = args[0];
            var outputDir = args[1];

            UnpackPackage(pakPath, outputDir, Mode);
        }

        public static FileBundle UnpackFile(string extension, Stream data, UnpackingMode mode)
        {
            if(extension != ".xnb" || mode == UnpackingMode.Raw)
            {
                return FileBundle.Single(data, extension);
            }
            else if (mode == UnpackingMode.DecompressedXNB)
            {
                return FileBundle.Single(XnbCompressor.Decompress(data), ".xnb");
            }
            else if (mode == UnpackingMode.Converted)
            {
                var converter = new XnbConverter();
                var outputBundle = converter.Convert(data);
                var formatName = converter.HeaderValid ? converter.FileType.Name.Replace("Reader", "") : "";
                if (converter.Converted)
                {
                    extension = converter.FormatConverter!.FileFormat;
                    var storageTypeName = (outputBundle.Files.Count > 1 ? "bundle" : "file");
                    Console.WriteLine($"  Format {formatName} converted into {extension} {storageTypeName}.");
                }
                else
                {
                    Console.WriteLine(
                        converter.HeaderValid 
                        ? $"  Unknown format {formatName}."
                        : $"  Not a valid XNB file."
                    );
                }
                return outputBundle;
            }
            else
            {
                return new FileBundle();
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
                    using var outputBundle = UnpackFile(extension, pakFile.Data, mode);

                    outputBundle.BundlePath = Path.Combine(outputDir, pakFile.Path + extension);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputBundle.BundlePath) ?? "");

                    foreach (var outputFile in outputBundle.Files)
                    {
                        using var fileOutputStream = File.Open(outputBundle.BundlePath + outputFile.Extension, FileMode.Create);
                        outputFile.Data.CopyTo(fileOutputStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to unpack {pakFile.Path} - {ex.Message}");
                }
                filesDone++;
            }
        }
    }
}
