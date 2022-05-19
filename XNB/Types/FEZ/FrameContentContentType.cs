using System.Drawing;
using FezEngine.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FEZRepacker.XNB.Types.XNA
{
    class FrameContentContentType : XNBContentType<FrameContent>
    {
        public FrameContentContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "FezEngine.Readers.FrameReader";

        public override object Read(BinaryReader reader)
        {
            FrameContent frame = new FrameContent();

            frame.Duration = Converter.ReadType<TimeSpan>(reader);
            frame.Rectangle = Converter.ReadType<Rectangle>(reader);

            return frame;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            FrameContent frame = (FrameContent)data;

            Converter.WriteType(frame.Duration, writer);
            Converter.WriteType(frame.Rectangle, writer);

        }
    }
}
