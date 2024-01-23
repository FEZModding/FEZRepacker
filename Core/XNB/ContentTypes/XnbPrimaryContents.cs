namespace FEZRepacker.Core.XNB.ContentTypes
{
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
            return List.Where(i => i.PrimaryContentType.Name.Equals(qualifier)).FirstOrDefault();
        }

        public static XnbPrimaryContentIdentity? FindByType(Type type)
        {
            return List.Where(i => i.PrimaryContentType.ContentType == type).FirstOrDefault();
        }
    }
}
