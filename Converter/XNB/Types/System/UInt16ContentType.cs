using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class UInt16ContentType : XnbContentType<ushort>
    {
        public UInt16ContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.UInt16Reader";

        public override object Read(BinaryReader reader)
        {
            return (object)reader.ReadUInt16();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((ushort)data);
        }
    }
}
