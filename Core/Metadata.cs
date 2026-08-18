using System.Reflection;

namespace FEZRepacker.Core
{
    public static class Metadata
    {
        public static readonly string Version;

        static Metadata()
        {
            var version = typeof(Metadata).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "0.0.0";

            var shaIndex = version.IndexOf('+');
            if (shaIndex >= 0)
            {
                version = version.Substring(0, Math.Min(shaIndex + 8, version.Length));
            }

            Version = $"FEZRepacker {version} by Krzyhau & FEZModding Team";
        }
    }
}
