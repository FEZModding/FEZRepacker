using System.CommandLine;

using FEZRepacker.Core.Conversion;

using static FEZRepacker.Interface.Actions.UnpackAction;
using static FEZRepacker.Interface.CommandLineOptions;

namespace FEZRepacker.Interface.Actions
{
    internal class UnpackGameAction : ICommandLineAction
    {
        public string Name => "--unpack-fez-content";

        public string[] Aliases => ["-g"];

        public string Description => 
            "Unpacks and converts all game assets into specified directory";

        public Argument[] Arguments => [_fezContentDirectory, _destinationDirectory];

        public Option[] Options => [UseArtObjectLegacyBundles, UseTrileSetLegacyBundles];

        private readonly Argument<FileInfo> _fezContentDirectory = new("fez-content-directory")
        {
            Description = "Source path of the content directory (usually, it's 'Content' in the game's directory)"
        };

        private readonly Argument<DirectoryInfo> _destinationDirectory = new("destination-directory")
        {
            Description = "Target path of the destination directory (creates one if doesn't exist)"
        };

        private static readonly string[] KnownPaks =
        [
            "Essentials.pak",
            "Music.pak",
            "Other.pak",
            "Updates.pak"
        ];
        
        private const string MusicPak = "Music.pak";
        
        public void Execute(ParseResult result)
        {
            var fezContentDirectory = result.GetRequiredValue(_fezContentDirectory);
            var destinationDirectory = result.GetRequiredValue(_destinationDirectory);

            var packagePaths = KnownPaks
                .Select(pak => new FileInfo(Path.Combine(fezContentDirectory.FullName, pak)))
                .ToArray();

            foreach (var packagePath in packagePaths)
            {
                if (!packagePath.Exists)
                {
                    throw new Exception($"Given directory is not FEZ's Content directory (missing {packagePath}).");
                }
            }

            var settings = new FormatConverterSettings
            {
                UseLegacyArtObjectBundle = result.GetValue(UseArtObjectLegacyBundles),
                UseLegacyTrileSetBundle = result.GetValue(UseTrileSetLegacyBundles)
            };

            foreach (var packagePath in packagePaths)
            {
                var actualOutputDir = destinationDirectory;
                if (packagePath.FullName.EndsWith(MusicPak))
                {
                    // Special Repacker behaviour - instead of dumping music tracks in
                    // the same folder as other assets, put them in separate music folder.
                    actualOutputDir = new DirectoryInfo(Path.Combine(destinationDirectory.FullName, "music"));
                }
                UnpackPackage(packagePath, actualOutputDir, UnpackingMode.Converted, settings);
            }
        }
    }
}
