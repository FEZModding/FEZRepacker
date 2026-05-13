using FEZRepacker.Core.Conversion;

namespace FEZRepacker.Interface
{
    public static class FormatConverterSettingsFlags
    {
        private const string UseTrixelArtBundle = "use-trixel-art-bundle";
        
        public static readonly CommandLineArgument[] Arguments =
        [
            new (UseTrixelArtBundle, ArgumentType.Flag)
        ];
        
        public static FormatConverterSettings ReadFromArguments(Dictionary<string, string> args) => new()
        {
            UseTrixelArtBundle = args.ContainsKey(UseTrixelArtBundle),
        };
    }
}