using System.CommandLine;

using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Interface.Actions
{
    internal class ListPackageContentAction : ICommandLineAction
    {
        public string Name => "--list";
        
        public string[] Aliases => ["-l"];
        
        public string Description => "Lists all files contained withing given .PAK package";

        public Argument[] Arguments => [_pakFile];
        
        public Option[] Options => [];

        private readonly Argument<FileInfo> _pakFile = new("pak-file")
        {
            Description = "The PAK file to use for listing files."
        };

        public void Execute(ParseResult result)
        {
            var pakFile = result.GetRequiredValue(_pakFile);
            var pakPackage = PakPackage.ReadFromFile(pakFile.FullName);

            Console.WriteLine($"PAK package \"{pakFile}\" with {pakPackage.Entries.Count} files.");
            Console.WriteLine();

            foreach (var entry in pakPackage.Entries)
            {
                var extension = entry.FindExtension();
                var typeText = extension.Length == 0 ? "unknown" : extension;
                Console.WriteLine($"{entry.Path} ({typeText} file, size: {entry.Length} bytes)");
            }
        }
    }
}
