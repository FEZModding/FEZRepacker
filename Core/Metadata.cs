using FEZRepacker.Core.XNB;

namespace FEZRepacker.Core
{
    public static class Metadata
    {
        public static readonly string Version;

        static Metadata()
        {
            var version = typeof(XnbSerializer).Assembly.GetName().Version?.ToString() ?? "";
            version = string.Join(".", version.Split('.').Take(3));
            Version = $"FEZRepacker {version} by Krzyhau & FEZModding Team";
        }
    }
}