using FEZRepacker.Converter.XNB.Formats;
using System.Drawing;

namespace FEZRepacker.Converter.XNB.Types.XNA
{
    internal class RectangleContentType : XnbContentType<Rectangle>
    {
        public RectangleContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.RectangleReader";

        public override object Read(BinaryReader reader)
        {
            return new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Rectangle rect = (Rectangle)data;

            writer.Write(rect.X);
            writer.Write(rect.Y);
            writer.Write(rect.Width);
            writer.Write(rect.Height);
        }
    }
}
