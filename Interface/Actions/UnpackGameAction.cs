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
        
        public string Name => "--unpack-fez-content";

        public string[] Aliases => new[] { "-g" };

        public string Description => 
            "Unpacks and converts all game assets into specified directory (creates one if doesn't exist).";

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument(FezContentDirectory),
            new CommandLineArgument(DestinationFolder),
            new CommandLineArgument(UseLegacyAo, ArgumentType.Flag),
            new CommandLineArgument(UseLegacyTs, ArgumentType.Flag)
        };

        public void Execute(Dictionary<string, string> args)
        {
            var contentPath = args[FezContentDirectory];
            var outputDir = args[DestinationFolder];

            var packagePaths = Directory.GetFiles(contentPath, "*.pak").OrderBy(f => f).ToList();
            Console.WriteLine($"Found directories: {string.Join(", ", packagePaths.Select(Path.GetFileName))}");
            
            var updatesPak = packagePaths.FirstOrDefault(f => f.EndsWith("Updates.pak"));
            if (!string.IsNullOrEmpty(updatesPak))
            {
                // Files from this directory are overwritten on top of existing ones, so process them last.
                packagePaths.Remove(updatesPak);
                packagePaths.Add(updatesPak);
            }
            else
            {
                Console.WriteLine("\n  WARNING! Updates.pak directory was not found.");
                Console.WriteLine("  Only older versions of the game (usually released before the current 1.12)");
                Console.WriteLine("  may not have this directory!\n");
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
