using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class CharContentType : XnbContentType<char>
    {
        public CharContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.CharReader";

        public override object Read(BinaryReader reader)
        {
            return (object)reader.ReadChar();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((char)data);
        }
    }
}
