using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

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
                    using var fileStream = file.Open();
                    bool written = writer.WriteFile(file.Path, fileStream, filterExtension: file.FindExtension());
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

        private void SortBundlesToPreventInvalidOrdering(ref List<FileBundle> fileBundles)
        {
            // Occasionally, on the process of repacking, we'll need to store multiple files with the same name.
            // This happens in the base game for effect files stored in Updates.pak. However, the game doesn't
            // support such behaviour, and attempts to perform operations only on the last encountered file,
            // which can create issues when files are ordered incorrectly. Putting file bundles without any
            // converters available first should solve the issue.

            fileBundles.Sort((a, b) =>
            {
                var converterA = FormatConverters.FindByExtension(a.MainExtension);
                var converterB = FormatConverters.FindByExtension(b.MainExtension);

                if (converterA != null && converterB == null) return 1;
                if (converterA == null && converterB != null) return -1;
                return a.BundlePath.CompareTo(b.BundlePath);
            });
        }

        public void Execute(string[] args)
        {
            string inputPath = args[0];
            string outputPackagePath = args[1];

            var fileBundlesToAdd = FileBundle.BundleFilesAtPath(inputPath);
            SortBundlesToPreventInvalidOrdering(ref fileBundlesToAdd);

            using var tempPak = new TemporaryPak(outputPackagePath);

            ConvertToXnbAction.PerformBatchConversion(fileBundlesToAdd, (path, extension, stream, converted) =>
            {
                if (!converted)
                {
                    Console.WriteLine($"  Packing raw file {path}{extension}...");
                }
                tempPak.Writer.WriteFile(path, stream, extension);
            });


            if (args.Length > 2)
            {
                IncludePackageIntoWriter(args[2], tempPak.Writer);
            }

            Console.WriteLine($"Packed {tempPak.Writer.FileCount} assets into {outputPackagePath}...");
        }
    }
}