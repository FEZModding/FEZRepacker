using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class BooleanContentType : XnbContentType<bool>
    {
        public BooleanContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.BooleanReader";

        public override object Read(BinaryReader reader)
        {
            return (object)reader.ReadBoolean();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((bool)data);
        }
    }
}