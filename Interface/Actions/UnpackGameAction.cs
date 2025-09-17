using static FEZRepacker.Interface.Actions.UnpackAction;

namespace FEZRepacker.Interface.Actions
{
    internal class UnpackGameAction : CommandLineAction
    {
        public string Name => "--unpack-fez-content";

        public string[] Aliases => new[] { "-g" };

        public string Description => 
            "Unpacks and converts all game assets into specified directory (creates one if doesn't exist).";

        public CommandLineArgument[] Arguments => new[] {
            new CommandLineArgument("fez-content-directory"),
            new CommandLineArgument("destination-folder")
        };

        public void Execute(Dictionary<string, string> args)
        {
            var contentPath = args["fez-content-directory"];
            var outputDir = args["destination-folder"];

            var packagePaths = new string[] { "Essentials.pak", "Music.pak", "Other.pak", "Updates.pak" }
                .Select(path => Path.Combine(contentPath, path)).ToArray();

            foreach (var packagePath in packagePaths)
            {
                if (!File.Exists(packagePath))
                {
                    throw new Exception($"Given directory is not FEZ's Content directory (missing {Path.GetFileName(packagePath)}).");
                }
            }

            foreach (var packagePath in packagePaths)
            {
                var actualOutputDir = outputDir;
                if (packagePath.EndsWith("Music.pak"))
                {
                    // Special Repacker behaviour - instead of dumping music tracks in
                    // the same folder as other assets, put them in separate music folder.
                    actualOutputDir = Path.Combine(outputDir, "music");
                }
                UnpackPackage(packagePath, actualOutputDir, UnpackingMode.Converted);
            }
        }
    }
}
