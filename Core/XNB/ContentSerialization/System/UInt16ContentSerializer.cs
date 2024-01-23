namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class UInt16ContentSerializer : XnbContentSerializer<ushort>
    {
        public UInt16ContentSerializer() : base() { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.UInt16Reader";

        public override object Deserialize(XnbContentReader reader)
        {
            return (object)reader.ReadUInt16();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            writer.Write((ushort)data);
        }
    }
}
