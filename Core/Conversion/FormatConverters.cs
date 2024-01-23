using FEZRepacker.Core.Conversion.Formats;
using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Core.Conversion
{
    /// <summary>
    /// Contains a statically declared list of all <see cref="FormatConverter"/>
    /// structures needed to convert all primary content types of FEZ into readable formats.
    /// </summary>
    public static class FormatConverters
    {
        public static readonly List<FormatConverter> List = new()
        {
            new AnimatedTextureConverter(),
            new ArtObjectConverter(),
            new EffectConverter(),
            new LevelConverter(),
            new MapTreeConverter(),
            new NpcMetadataConverter(),
            new SkyConverter(),
            new SoundEffectConverter(),
            new SpriteFontConverter(),
            new TextStorageConverter(),
            new TextureConverter(),
            new TrackedSongConverter(),
            new TrileSetConverter()
        };

        public static FormatConverter? FindByExtension(string extension)
        {
            return List.FirstOrDefault(x => x.FileFormat == extension);
        }

        public static FormatConverter? FindForFileBundle(FileBundle bundle)
        {
            return FindByExtension(bundle.MainExtension);
        }

        public static FormatConverter? FindForType(Type type)
        {
            return List.FirstOrDefault(x => x.FormatType == type);
        }
    }
}
