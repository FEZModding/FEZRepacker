using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.TrileSet;

namespace FEZRepacker.Core.Conversion
{
    /// <summary>
    /// Contains data for changing the behavior of the converter
    /// </summary>
    public struct FormatConverterSettings()
    {
        /// <summary>
        /// By default, the <see href="https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html">glTF</see> all-in-one
        /// format is used for transmitting and editing <see cref="ArtObject"/> and <see cref="TrileSet"/> properties.
        /// If the flag is true, the converter will use a legacy bundle with separate files.
        /// </summary>
        public bool UseTrixelArtBundle = false;

        /// <summary>
        /// By default, <see cref="AnimatedTexture"/> is converted into GIF animation file. This is a lossy conversion,
        /// dropping original atlas texture arrangement and leading to minor precision loss to color and frame duration.
        /// If the flag is true, the converter will export animations into a bundle of atlas texture and JSON data.
        /// </summary>
        public bool UseAnimationSheet = false;
    }
}