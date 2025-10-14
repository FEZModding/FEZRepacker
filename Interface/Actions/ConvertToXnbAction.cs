using System.CommandLine;

using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

namespace FEZRepacker.Interface.Actions
{
    internal class ConvertToXnbAction : ICommandLineAction
    {
        public string Name => "--convert-to-xnb";

        public string[] Aliases => ["-X"];

        public string Description =>
            "Attempts to convert given input into XNB file(s) and save it at given output directory";

        public Argument[] Arguments => [_inputSource];

        public Option[] Options => [_outputDirectory];
        
        private readonly Argument<FileSystemInfo> _inputSource = new("input-source")
        {
            Description = "Given input to convert (this can be a path to a single file or an entire directory).\n"
                + "  If input is a directory, dumps all converted files in specified path recursively."
        };
        
        private readonly Option<DirectoryInfo> _outputDirectory = new("output-directory")
        {
            Description = "Output directory for saving deconverted XNB file(s).\n"
                + "  If output directory is not given, outputs next to the input file(s)."
        };

        public delegate void ConversionFunc(string path, string extension, Stream stream, bool converted);

        public static void PerformBatchConversion(List<FileBundle> fileBundles, ConversionFunc processFileFunc)
        {
            Console.WriteLine($"Converting {fileBundles.Count} assets...");
            var filesDone = 0;
            foreach (var fileBundle in fileBundles)
            {
                Console.WriteLine($"({filesDone + 1}/{fileBundles.Count}) {fileBundle.BundlePath}");

                try
                {
                    object convertedData = FormatConversion.Deconvert(fileBundle)!;
                    var data = XnbSerializer.Serialize(convertedData);

                    Console.WriteLine($"  Format {fileBundle.MainExtension} deconverted into {convertedData.GetType().Name}.");
                    processFileFunc(fileBundle.BundlePath, ".xnb", data, true);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"  Unable to convert asset: {ex.Message}");
                    foreach (var file in fileBundle.Files)
                    {
                        file.Data.Seek(0, SeekOrigin.Begin);
                        var ext = fileBundle.MainExtension + file.Extension;
                        processFileFunc(fileBundle.BundlePath, ext, file.Data, false);
                    }
                }
                filesDone++;

                fileBundle.Dispose();
            }
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

            var fileBundles = FileBundle.BundleFilesAtPath(inputSource.FullName);
            Console.WriteLine($"Found {fileBundles.Count} file bundles.");

            PerformBatchConversion(fileBundles, (path, extension, stream, converted) =>
            {
                if (!converted) return;
                
                var assetOutputFullPath = Path.Combine(outputPath, $"{path}{extension}");

                Directory.CreateDirectory(Path.GetDirectoryName(assetOutputFullPath) ?? "");

                using var assetFile = File.Create(assetOutputFullPath);
                stream.CopyTo(assetFile);
            });
        }
    }
}
