using System.CommandLine;

using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Interface.Actions
{
    internal class PackAction : ICommandLineAction
    {
        public string Name => "--pack";
        
        public string[] Aliases => ["-p"];
        
        public string Description =>
            "Loads files, tries to deconvert them and pack into a destination .PAK file";

        public Argument[] Arguments => [_inputDirectory, _destinationPakFile];

        public Option[] Options => [_includePakFile];
        
        private readonly Argument<DirectoryInfo> _inputDirectory = new("input-directory")
        {
          Description = "Path of the input directory with files"
        };

        private readonly Argument<FileInfo> _destinationPakFile = new("destination-pak-file")
        {
            Description = "Path of the destination directory (creates one if doesn't exist)"
        };

        private readonly Option<FileInfo> _includePakFile = new("include-pak-file")
        {
            Description = "If it's provided, it'll add its content into the new .PAK package"
        };

        private class TemporaryPak : IDisposable
        {
            private readonly string _tempPath;
            
            private readonly string _resultPath;

            public readonly PakWriter Writer;

            public TemporaryPak(FileInfo file)
            {
                _tempPath = GetNewTempPackagePath();
                _resultPath = file.FullName;

                var tempPakStream = File.Open(_tempPath, FileMode.Create);
                Writer = new PakWriter(tempPakStream);
            }

            private static string GetNewTempPackagePath()
            {
                return Path.GetTempPath() + "repacker_pak_" + Guid.NewGuid() + ".pak";
            }
            
            public void Dispose()
            {
                Writer.Dispose();
                File.Move(_tempPath, _resultPath, overwrite: true);
            }
        }

        private static void IncludePackageIntoWriter(FileInfo includePackage, PakWriter writer)
        {
            try
            {
                using var includePackageReader = PakReader.FromFile(includePackage.FullName);
                foreach (var file in includePackageReader.ReadFiles())
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
                Console.Error.WriteLine($"Could not fully load included package: {e.Message}");
            }
        }

        private static void SortBundlesToPreventInvalidOrdering(ref List<FileBundle> fileBundles)
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
                return String.Compare(a.BundlePath, b.BundlePath, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        public void Execute(ParseResult result)
        {
            var inputDirectory = result.GetRequiredValue(_inputDirectory);
            var destinationPakFile = result.GetRequiredValue(_destinationPakFile);

            var fileBundlesToAdd = FileBundle.BundleFilesAtPath(inputDirectory.FullName);
            SortBundlesToPreventInvalidOrdering(ref fileBundlesToAdd);

            using var tempPak = new TemporaryPak(destinationPakFile);

            ConvertToXnbAction.PerformBatchConversion(fileBundlesToAdd, (path, extension, stream, converted) =>
            {
                if (!converted)
                {
                    Console.WriteLine($"  Packing raw file {path}{extension}...");
                }
                tempPak.Writer.WriteFile(path, stream, extension);
            });

            var includePakFile = result.GetValue(_includePakFile);
            if (includePakFile != null)
            {
                IncludePackageIntoWriter(includePakFile, tempPak.Writer);
            }

            Console.WriteLine($"Packed {tempPak.Writer.FileCount} assets into {destinationPakFile}...");
        }
    }
}