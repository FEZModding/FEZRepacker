
using FEZRepacker.Converter.FileSystem;

namespace FEZRepacker.Interface.Actions
{
    internal class PackAction : CommandLineAction
    {
        public string Name => "--pack";
        public string[] Aliases => new[] { "-p" };
        public string Description =>
            "Loads files from given input directory path, tries to deconvert them and pack into a destination " +
            ".PAK file with given path. If include .PAK path is provided, it'll add its content into the new .PAK package.";
        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument("input-directory-path"),
            new CommandLineArgument("destination-pak-path"),
            new CommandLineArgument("include-pak-path", true)
        };

        private class TemporaryPak : IDisposable
        {
            private readonly string tempPath;
            private readonly string resultPath;

            public PakWriter Writer { get; private set; }

            public TemporaryPak(string finalPath)
            {
                tempPath = GetNewTempPackagePath();
                resultPath = finalPath;

                var tempPakStream = File.Open(tempPath, FileMode.Create);
                Writer = new PakWriter(tempPakStream);
            }

            private static string GetNewTempPackagePath()
            {
                return Path.GetTempPath() + "repacker_pak_" + Guid.NewGuid().ToString() + ".pak";
            }
            public void Dispose()
            {
                Writer.Dispose();
                File.Move(tempPath, resultPath, overwrite: true);
            }
        }

        private void IncludePackageIntoWriter(string includePackagePath, PakWriter writer)
        {
            try
            {
                using var includePackage = PakReader.FromFile(includePackagePath);
                foreach (var file in includePackage.ReadFiles())
                {
                    bool written = writer.WriteFile(file.Path, file.Data, filterExtension: file.DetectedFileExtension);
                    if (!written)
                    {
                        Console.WriteLine($"Skipping asset from included package {file.Path}, as it's already in the output package.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not fully load included package: $e", e.Message);
            }
        }

        public void Execute(string[] args)
        {
            string inputPath = args[0];
            string outputPackagePath = args[1];

            var fileBundlesToAdd = FileBundle.BundleFilesAtPath(inputPath);

            using var tempPak = new TemporaryPak(outputPackagePath);

            ConvertToXnbAction.PerformBatchConversion(fileBundlesToAdd, (path, extension, stream, converted) =>
            {
                if (!converted)
                {
                    Console.WriteLine($"  Packing raw file {path}{extension}...");
                }
                tempPak.Writer.WriteFile(path, stream, extension);
            });

            
            if(args.Length > 2)
            {
                IncludePackageIntoWriter(args[2], tempPak.Writer);
            }

            Console.WriteLine($"Packed {tempPak.Writer.FileCount} assets into {outputPackagePath}...");
        }
    }
}
