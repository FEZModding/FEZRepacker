using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

namespace FEZRepacker.Interface.Actions
{
    internal class ListPackageContentAction : CommandLineAction
    {
        private const string PakPath = "pak-path";
        
        public string Name => "--list";
        public string[] Aliases => new[] { "-l" };
        public string Description => "Lists all files contained withing given .PAK package.";
        public IEnumerable<CommandLineArgument> Arguments => [
            new(PakPath)
        ];

        public void Execute(Dictionary<string, string> args)
        {
            var pakPath = args[PakPath];
            var pakPackage = PakPackage.ReadFromFile(pakPath);

            Console.WriteLine($"PAK package \"{pakPath}\" with {pakPackage.Entries.Count} files.");
            Console.WriteLine();

            foreach (var entry in pakPackage.Entries)
            {
                var extension = entry.FindExtension();
                if (extension == ".xnb")
                {
                    using var pakFile = entry.Open();
                    string assetTypeName;
                    try
                    {
                        var assetType = XnbSerializer.DeserializePrimaryContentTypeOnly(pakFile);
                        assetTypeName = assetType.Name;
                    }
                    catch
                    {
                        assetTypeName = "unknown";
                    }
                    
                    Console.WriteLine($"{entry.Path} ({assetTypeName} XNB asset, size: {entry.Length} bytes)");
                    continue;
                }
                
                var typeText = extension.Length == 0 ? "unknown" : extension;
                Console.WriteLine($"{entry.Path} ({typeText} file, size: {entry.Length} bytes)");
            }
        }
    }
}
