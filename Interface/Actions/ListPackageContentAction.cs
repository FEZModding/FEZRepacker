using FEZRepacker.Converter.FileSystem;

namespace FEZRepacker.Interface.Actions
{
    internal class ListPackageContentAction : CommandLineAction
    {
        public string Name => "--list";
        public string[] Aliases => new[] { "-l" };
        public string Description => "Lists all files contained withing given .PAK package.";
        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument("pak-path")
        };

        public void Execute(string[] args)
        {
            var pakPath = args[0];

            using var pakReader = PakReader.FromFile(pakPath);

            Console.WriteLine($"PAK package \"{pakPath}\" with {pakReader.FileCount} files.");
            Console.WriteLine();

            foreach (var item in pakReader.ReadFiles())
            {
                var typeText = item.DetectedFileExtension.Length == 0 ? "unknown" : item.DetectedFileExtension;
                Console.WriteLine($"{item.Path} ({typeText} file, size: {item.Size} bytes)");
            }
        }
    }
}
