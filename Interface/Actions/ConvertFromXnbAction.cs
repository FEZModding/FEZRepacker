using System.CommandLine;

using FEZRepacker.Core.Conversion;

using static FEZRepacker.Interface.CommandLineOptions;

namespace FEZRepacker.Interface.Actions
{
    internal class ConvertFromXnbAction : ICommandLineAction
    {
        public string Name => "--convert-from-xnb";

        public string[] Aliases => ["-x"];

        public string Description =>
            "Attempts to convert given XNB input and save it at given output directory";

        public Argument[] Arguments => [_inputSource, _outputDirectory];

        public Option[] Options => [UseTrileSetLegacyBundles, UseArtObjectLegacyBundles];
        
        private readonly Argument<FileSystemInfo> _inputSource = new("input-source")
        {
            Description = "Given XNB input to convert (this can be a path to a single file or an entire directory).\n"
                          + "  If input is a directory, dumps all converted files in specified path recursively."
        };
        
        private readonly Argument<DirectoryInfo> _outputDirectory = new("output-directory")
        {
            Arity = ArgumentArity.ZeroOrOne,
            Description = "Output directory for saving converted file(s).\n"
                          + "  If output directory is not given, outputs next to the input file(s)."
        };

        private static List<string> FindXnbFilesAtPath(FileSystemInfo fileSystem)
        {
            if (fileSystem is DirectoryInfo { Exists: true } directoryInfo)
            {
                var xnbFiles = directoryInfo.GetFiles("*.xnb", SearchOption.AllDirectories);
                Console.WriteLine($"Found {xnbFiles.Length} XNB files in given directory.");
                return xnbFiles.Select(f => f.FullName).ToList();
            }

            if (fileSystem is not FileInfo fileInfo)
            {
                return [fileSystem.FullName];
            }

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Specified input path does not lead to any file or a directory");
            }
            
            return fileInfo.Extension != ".xnb"
                ? throw new Exception("An input file must be an .XNB file.")
                : [fileSystem.FullName];
        }
        
        public void Execute(ParseResult result)
        {
            var inputSource = result.GetRequiredValue(_inputSource);
            var outputDirectory = result.GetValue(_outputDirectory);

            var outputPath = inputSource switch
            {
                FileInfo inputFile => inputFile.DirectoryName!,
                DirectoryInfo inputDirectory => inputDirectory.FullName,
                _ => throw new ArgumentException(nameof(inputSource))
            };

            if (outputDirectory != null)
            {
                if (!outputDirectory.Exists)
                {
                    outputDirectory.Create();
                }
                outputPath = outputDirectory.FullName;
            }

            var xnbFilesToConvert = FindXnbFilesAtPath(inputSource);
            Console.WriteLine($"Converting {xnbFilesToConvert.Count} XNB files...");

            var filesDone = 0;
            var settings = new FormatConverterSettings
            {
                UseLegacyArtObjectBundle = result.GetValue(UseArtObjectLegacyBundles),
                UseLegacyTrileSetBundle = result.GetValue(UseTrileSetLegacyBundles)
            };
            
            foreach (var xnbPath in xnbFilesToConvert)
            {
                Console.WriteLine($"({filesDone + 1}/{xnbFilesToConvert.Count}) {xnbPath}");

                using var xnbStream = File.OpenRead(xnbPath);

                using var outputBundle = UnpackAction.UnpackFile(".xnb", xnbStream, UnpackAction.UnpackingMode.Converted, settings);

                var relativePathRaw = xnbPath == inputSource.FullName
                    ? Path.GetFileName(xnbPath)
                    : Path.GetRelativePath(inputSource.FullName, xnbPath);
                
                var relativePath = relativePathRaw
                    .Replace("/", "\\")
                    .Replace(".xnb", "", StringComparison.InvariantCultureIgnoreCase);
                
                outputBundle.BundlePath = Path.Combine(outputPath, relativePath + outputBundle.MainExtension);
                var outputDirectoryPath = Path.GetDirectoryName(outputBundle.BundlePath) ?? "";

                Directory.CreateDirectory(outputDirectoryPath);
                foreach (var outputFile in outputBundle.Files)
                {
                    using var fileOutputStream = File.Open(outputBundle.BundlePath + outputFile.Extension, FileMode.Create);
                    outputFile.Data.CopyTo(fileOutputStream);
                }

                filesDone++;
            }
        }
    }
}
