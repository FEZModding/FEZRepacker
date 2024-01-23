using FEZRepacker.Core.XNB.ContentSerialization;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Structure storing a description of a primary content type that
    /// can be found in an XNB file. <see cref="ContentTypesFactory"/> is expected
    /// to be overloaded to return a list of <see cref="XnbContentSerializer"/>s
    /// representing this content type (with the first one in the list being 
    /// the primary type).
    /// </summary>
    /// <remarks>
    /// Each content type uses more-or-less the same types. Instead of generating
    /// them dynamically, each primary content type has its own identity structure
    /// defining all types it could possibly used. This can create disparity to the 
    /// original XNB asset format, but FEZ is fine with it, and we gain some 
    /// performance from it.
    /// </remarks>
    internal abstract class XnbPrimaryContentIdentity
    {
        protected abstract List<XnbContentSerializer> ContentTypesFactory { get; }

        public readonly List<XnbContentSerializer> ContentTypes;
        public readonly List<XnbContentSerializer> PublicContentTypes;
        public XnbContentSerializer PrimaryContentType => PublicContentTypes[0];
        public string FormatName => PrimaryContentType.Name.Name.Replace("Reader", "");

        public XnbPrimaryContentIdentity()
        {
            ContentTypes = ContentTypesFactory;
            PublicContentTypes = ContentTypes.Where(t => !t.IsPrivate).ToList();
        }

    }
}
