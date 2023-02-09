using System.Linq;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal static class XnbFormatList
    {
        private static List<XnbFormatConverter> converters = new List<XnbFormatConverter>();

        static XnbFormatList()
        {
            Add(new TextStorageConverter());
            Add(new TextureConverter());
            Add(new AnimatedTextureConverter());
            Add(new LevelConverter());
            Add(new SoundEffectConverter());
            Add(new EffectConverter());
            //Add(new ArtObjectConverter());
        }

        private static void Add(XnbFormatConverter converter)
        {
            converters.Add(converter);
        }

        public static XnbFormatConverter? FindByExtension(string extension)
        {
            return converters.Where(c => c.FileFormat.Equals(extension, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public static XnbFormatConverter? FindByQualifier(XnbAssemblyQualifier qualifier)
        {
            return converters.Where(c => c.PrimaryContentType.Name.Equals(qualifier)).FirstOrDefault();
        }
    }
}
