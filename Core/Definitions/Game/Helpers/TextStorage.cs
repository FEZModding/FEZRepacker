namespace FEZRepacker.Core.Definitions.Game.Helpers
{
    [XnbReaderType(
        "Microsoft.Xna.Framework.Content.DictionaryReader`2[[" +
                    "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]," +
                    "[System.Collections.Generic.Dictionary`2[[" +
                        "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]," +
                        "[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" +
                    "]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" +
                "]]")]
    // Wrapper class for text storage asset type. Helps with its management.
    // Original game reads dictionary type directly.
    public class TextStorage
    {
        [XnbProperty(SkipIdentifier = true, UseConverter = true)]
        public IDictionary<string, IDictionary<string, string>> AllResources { get; set; }
    }
}
