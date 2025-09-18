
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

namespace FEZRepacker.Interface.Actions
{
    internal class ConvertFromXnbAction : CommandLineAction
    {
        private const string XnbInput = "xnb-input";
        
        private const string FileOutput = "file-output";
        
        public string Name => "--convert-from-xnb";

        public string[] Aliases => new[] { "-x" };

        public string Description =>
            "Attempts to convert given XNB input (this can be a path to a single asset or an entire directory) " +
            "and save it at given output directory. If input is a directory, dumps all converted files in specified " +
            "path recursively. If output directory is not given, outputs next to the input file(s).";

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument(XnbInput),
            new CommandLineArgument(FileOutput, ArgumentType.OptionalPositional)
        };

        private List<string> FindXnbFilesAtPath(string path)
        {
            if (Directory.Exists(path))
            {
                var xnbFiles = Directory.GetFiles(path, "*.xnb", SearchOption.AllDirectories).ToList();
                Console.WriteLine($"Found {xnbFiles.Count()} XNB files in given directory.");
                return xnbFiles;
            }
            else if (File.Exists(path))
            {
                if (Path.GetExtension(path) != ".xnb")
                {
                    throw new Exception("An input file must be an .XNB file.");
                }
                return new List<string> { path };
            }
            else
            {
                throw new FileNotFoundException("Specified input path does not lead to any file or a directory");
            }
        }
        
        public void Execute(Dictionary<string, string> args)
        {
            var inputPath = args[XnbInput];
            var outputPath = args.GetValueOrDefault(FileOutput, inputPath);

            if (File.Exists(outputPath))
            {
                outputPath = Path.GetDirectoryName(outputPath) ?? "";
            }
            Directory.CreateDirectory(outputPath);

            var xnbFilesToConvert = FindXnbFilesAtPath(inputPath);

            Console.WriteLine($"Converting {xnbFilesToConvert.Count()} XNB files...");

            var filesDone = 0;
            foreach (var xnbPath in xnbFilesToConvert)
            {
                Console.WriteLine($"({filesDone + 1}/{xnbFilesToConvert.Count}) {xnbPath}");

                using var xnbStream = File.OpenRead(xnbPath);

                using var outputBundle = UnpackAction.UnpackFile(".xnb", xnbStream, UnpackAction.UnpackingMode.Converted);

                var relativePath = Path.GetRelativePath(inputPath, xnbPath)
                    .Replace("/", "\\")
                    .Replace(".xnb", "", StringComparison.InvariantCultureIgnoreCase);
                outputBundle.BundlePath = Path.Combine(outputPath, relativePath + outputBundle.MainExtension);
                var outputDirectory = Path.GetDirectoryName(outputBundle.BundlePath) ?? "";
                

                Directory.CreateDirectory(outputDirectory);
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
