using FEZRepacker.Core.XNB.ContentSerialization;

namespace FEZRepacker.Core.XNB.ContentTypes
{
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
