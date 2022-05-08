using FEZRepacker.XNB.Types;
using FEZRepacker.XNB.Types.System;

namespace FEZRepacker.XNB.Converters.Files
{
    class TextStorageConverter : YamlStorageConverter<Dictionary<string, Dictionary<string, string>>>
    {
        public override XNBContentType[] Types => new XNBContentType[]
        {
            new DictionaryContentType<string, Dictionary<string, string>>(this),
            new StringContentType(this),
            new DictionaryContentType<string, string>(this)
        };
        public override string FileFormat => "fezdata";
    }
}
