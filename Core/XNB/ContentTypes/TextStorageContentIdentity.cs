using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class TextStorageContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new TextStorageContentSerializer().MarkPrivate(),
            new DictionaryContentSerializer<string, IDictionary<string, string>>(),
            new StringContentSerializer(),
            new DictionaryContentSerializer<string, string>()
        };
    }
}
