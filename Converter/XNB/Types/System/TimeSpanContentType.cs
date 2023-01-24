using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class TimeSpanContentType : XnbContentType<TimeSpan>
    {
        public TimeSpanContentType(XnbFormatConverter converter) : base(converter) { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.TimeSpanReader";

        public override object Read(BinaryReader reader)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write(((TimeSpan)data).Ticks);
        }
    }
}
