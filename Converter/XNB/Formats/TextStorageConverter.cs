using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class TextStorageConverter : JsonStorageConverter<Dictionary<string, Dictionary<string, string>>>
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new DictionaryContentType<string, Dictionary<string, string>>(this),
            new StringContentType(this),
            new DictionaryContentType<string, string>(this)
        };
        public override string FileFormat => ".fezdata";
    }
}