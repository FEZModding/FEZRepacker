
using System.IO;

using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

namespace FEZRepacker.Interface.Actions
{
    internal class ConvertToXnbAction : CommandLineAction
    {
        public string Name => "--convert-to-xnb";

        public string[] Aliases => new[] { "-X" };

        public string Description =>
            "Attempts to convert given input (this can be a path to a single file or an entire directory) " +
            "into XNB file(s) and save it at given output directory. If input is a directory, dumps all converted files in" +
            "specified path recursively. If output directory is not given, outputs next to the input file(s).";

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument("file-input"),
            new CommandLineArgument("xnb-output", true)
        };


        public delegate void ConversionFunc(string path, string extension, Stream stream, bool converted);
        public static void PerformBatchConversion(List<FileBundle> fileBundles, ConversionFunc processFileFunc)
        {
            Console.WriteLine($"Converting {fileBundles.Count()} assets...");
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

        public void Execute(string[] args)
        {
            string inputPath = args[0];
            string outputPath = args.Length > 1 ? args[1] : inputPath;

            if (File.Exists(outputPath))
            {
                outputPath = Path.GetDirectoryName(outputPath) ?? "";
            }
            Directory.CreateDirectory(outputPath);

            var fileBundles = FileBundle.BundleFilesAtPath(inputPath);
            Console.WriteLine($"Found {fileBundles.Count()} file bundles.");


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
