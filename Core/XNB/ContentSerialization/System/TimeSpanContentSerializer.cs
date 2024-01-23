namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class TimeSpanContentSerializer : XnbContentSerializer<TimeSpan>
    {
        public TimeSpanContentSerializer() : base() { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.TimeSpanReader";

        public override object Deserialize(XnbContentReader reader)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            writer.Write(((TimeSpan)data).Ticks);
        }
    }
}
