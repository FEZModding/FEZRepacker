using FEZRepacker.Core.Conversion;

using static FEZRepacker.Interface.Actions.UnpackAction;

namespace FEZRepacker.Interface.Actions
{
    internal class UnpackGameAction : CommandLineAction
    {
        private const string FezContentDirectory = "fez-content-directory";
        private const string DestinationFolder = "destination-folder";
        private const string UseLegacyAo = "use-legacy-ao";
        private const string UseLegacyTs = "use-legacy-ts";
        private const string SkipUpdates = "skip-updates";
        
        public string Name => "--unpack-fez-content";

        public string[] Aliases => new[] { "-g" };

        public string Description => 
            "Unpacks and converts all game assets into specified directory (creates one if doesn't exist).";

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument(FezContentDirectory),
            new CommandLineArgument(DestinationFolder),
            new CommandLineArgument(UseLegacyAo, ArgumentType.Flag),
            new CommandLineArgument(UseLegacyTs, ArgumentType.Flag),
            new CommandLineArgument(SkipUpdates, ArgumentType.Flag)
        };

        public void Execute(Dictionary<string, string> args)
        {
            var contentPath = args[FezContentDirectory];
            var outputDir = args[DestinationFolder];

            var packagePaths = new string[] { "Essentials.pak", "Music.pak", "Other.pak", "Updates.pak" }
                .Select(path => Path.Combine(contentPath, path)).ToArray();

            foreach (var packagePath in packagePaths)
            {
                if (!File.Exists(packagePath))
                {
                    if (packagePath.EndsWith("Updates.pak") && args.ContainsKey(SkipUpdates))
                    {
                        // Older, pre-FNA releases may not have this PAK.
                        Console.WriteLine($"Skipping the {Path.GetFileName(packagePath)} directory.");
                        continue;
                    }
                    
                    throw new Exception($"Given directory is not FEZ's Content directory (missing {Path.GetFileName(packagePath)}).");
                }
            }
            
            var settings = new FormatConverterSettings
            {
                UseLegacyArtObjectBundle = args.ContainsKey(UseLegacyAo),
                UseLegacyTrileSetBundle = args.ContainsKey(UseLegacyTs)
            };

            foreach (var packagePath in packagePaths)
            {
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
