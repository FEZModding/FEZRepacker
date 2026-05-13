using FEZRepacker.Core.Conversion;

namespace FEZRepacker.Interface
{
    public static class FormatConverterSettingsFlags
    {
        private const string UseTrixelArtBundle = "use-trixel-art-bundle";
        private const string UseAnimationSheet = "use-animation-sheet";
        
        public static readonly CommandLineArgument[] Arguments =
        [
            new (UseTrixelArtBundle, ArgumentType.Flag),
            new (UseAnimationSheet, ArgumentType.Flag)
        ];
        
        public static FormatConverterSettings ReadFromArguments(Dictionary<string, string> args) => new()
        {
            UseTrixelArtBundle = args.ContainsKey(UseTrixelArtBundle),
            UseAnimationSheet = args.ContainsKey(UseAnimationSheet),
        };
    }
}