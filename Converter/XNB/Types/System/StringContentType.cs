using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class StringContentType : XnbContentType<string>
    {
        public StringContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.StringReader";

        public override object Read(BinaryReader reader)
        {
            return reader.ReadString();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((string)data);
        }

        public override bool IsEmpty(object data)
        {
            return ((string)data).Length == 0;
        }
    }
}