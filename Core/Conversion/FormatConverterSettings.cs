using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.TrileSet;

namespace FEZRepacker.Core.Conversion
{
    /// <summary>
    /// Contains data for changing the behavior of the converter
    /// </summary>
    public struct FormatConverterSettings
    {
        /// <summary>
        /// By default, the <see href="https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html">glTF</see>
        /// all-in-one format is used for transmitting and editing <see cref="ArtObject"/> properties.
        /// If the flag is true, the converter will use a legacy bundle with separate files.
        /// </summary>
        public bool UseLegacyArtObjectBundle;

        /// <summary>
        /// By default, the <see href="https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html">glTF</see>
        /// all-in-one format is used for transmitting and editing <see cref="TrileSet"/> properties.
        /// If the flag is true, the converter will use a legacy bundle with separate files.
        /// </summary>
        public bool UseLegacyTrileSetBundle;
    }
}