using System.Drawing;
using FezEngine.Structure;

namespace FEZRepacker.XNB.Types.XNA
{
    class RectangleContentReader : XNBContentType<Rectangle>
    {
        public RectangleContentReader(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.RectangleReader";

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
