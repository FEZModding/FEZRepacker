using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Interface.Actions
{
    internal class ListPackageContentAction : CommandLineAction
    {
        private const string PakPath = "pak-path";
        
        public string Name => "--list";
        public string[] Aliases => new[] { "-l" };
        public string Description => "Lists all files contained withing given .PAK package.";
        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument(PakPath)
        };

        public void Execute(Dictionary<string, string> args)
        {
            var pakPath = args[PakPath];
            var pakPackage = PakPackage.ReadFromFile(pakPath);

            Console.WriteLine($"PAK package \"{pakPath}\" with {pakPackage.Entries.Count} files.");
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
