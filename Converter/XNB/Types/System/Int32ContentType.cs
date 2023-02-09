using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class Int32ContentType : XnbContentType<int>
    {
        public Int32ContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.Int32Reader";

        public override object Read(BinaryReader reader)
        {
            return (object)reader.ReadInt32();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((int)data);
        }
    }
}