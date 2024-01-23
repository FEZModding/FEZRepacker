
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class TextStorageContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new DictionaryContentSerializer<string, Dictionary<string, string>>(),
            new StringContentSerializer(false),
            new DictionaryContentSerializer<string, string>()
        };
    }
}
