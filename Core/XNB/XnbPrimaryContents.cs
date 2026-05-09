using FEZRepacker.Core.XNB.ContentTypes;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Contains a statically declared list of all <see cref="XnbPrimaryContentIdentity"/>
    /// structures needed to parse all XNB files contained within FEZ.
    /// </summary>
    internal static class XnbPrimaryContents
    {
        public static readonly List<XnbPrimaryContentIdentity> List = new()
        {
            new AnimatedTextureContentIdentity(),
            new ArtObjectContentIdentity(),
            new EffectContentIdentity(),
            new LevelContentIdentity(),
            new MapContentIdentity(),
            new NpcMetadataContentIdentity(),
            new SkyContentIdentity(),
            new SoundEffectContentIdentity(),
            new SpriteFontContentIdentity(),
            new TextStorageContentIdentity(),
            new TextureContentIdentity(),
            new TrackedSongContentIdentity(),
            new TrileSetContentIdentity()
        };

        public static XnbPrimaryContentIdentity? FindByQualifier(XnbAssemblyQualifier qualifier)
        {
            return List.FirstOrDefault(contentIdentity => contentIdentity.PrimaryContentSerializer.Name.Equals(qualifier));
        }

        public static XnbPrimaryContentIdentity? FindByType(Type type)
        {
            var readerQualifier = XnbAssemblyQualifier.TryGetFromXnbReaderType(type);
            if (readerQualifier == null)
            {
                return null;
            }
            return FindByQualifier(readerQualifier.Value);
        }
    }
}
