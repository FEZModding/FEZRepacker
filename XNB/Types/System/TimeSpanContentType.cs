namespace FEZRepacker.XNB.Types.System
{
    class TimeSpanContentType : XNBContentType<TimeSpan>
    {
        public TimeSpanContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.TimeSpanReader";

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
