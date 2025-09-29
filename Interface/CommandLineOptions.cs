using System.CommandLine;

namespace FEZRepacker.Interface
{
    internal static class CommandLineOptions
    {
        public static readonly Option<bool> UseArtObjectLegacyBundles = new("--use-legacy-ao", "-lao")
        {
            Description = "Use a legacy bundle format with separate files instead of all-in-one glTF bundle for Art Objects."
        };
        
        public static readonly Option<bool> UseTrileSetLegacyBundles = new("--use-legacy-ts", "-lts")
        {
            Description = "Use a legacy bundle format with separate files instead of all-in-one glTF bundle for Trile Sets."
        };
    }
}