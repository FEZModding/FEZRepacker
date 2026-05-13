using FEZRepacker.Core.Conversion;

using static FEZRepacker.Interface.Actions.UnpackAction;

namespace FEZRepacker.Interface.Actions
{
    internal class UnpackGameAction : CommandLineAction
    {
        private static string[] ExpectedPackagesFileNames = ["Essentials.pak", "Music.pak", "Other.pak", "Updates.pak"];
        
        private const string FezContentDirectory = "fez-content-directory";
        private const string DestinationFolder = "destination-folder";
        
        public string Name => "--unpack-fez-content";

        public string[] Aliases => new[] { "-g" };

        public string Description => 
            "Unpacks and converts all game assets into specified directory (creates one if doesn't exist).";

        public IEnumerable<CommandLineArgument> Arguments => new[] {
            new CommandLineArgument(FezContentDirectory),
            new CommandLineArgument(DestinationFolder)
        }.Concat(FormatConverterSettingsFlags.Arguments);

        public void Execute(Dictionary<string, string> args)
        {
            var contentPath = args[FezContentDirectory];
            var outputDir = args[DestinationFolder];

            var settings = FormatConverterSettingsFlags.ReadFromArguments(args);

            foreach (var packageFileName in ExpectedPackagesFileNames)
            {
                var packagePath = Path.Combine(contentPath, packageFileName);

                if (!File.Exists(packagePath))
                {
                    Console.WriteLine($"Warning: Expected package file {packageFileName} not found.");
                    continue;
                }
                
                var actualOutputDir = outputDir;
                if (packagePath.EndsWith("Music.pak"))
                {
                    // Special Repacker behaviour - instead of dumping music tracks in
                    // the same folder as other assets, put them in separate music folder.
                    actualOutputDir = Path.Combine(outputDir, "music");
                }
                UnpackPackage(packagePath, actualOutputDir, UnpackingMode.Converted, settings);
            }
        }
    }
}
