namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class Int32ContentSerializer : XnbContentSerializer<int>
    {
        public Int32ContentSerializer() : base() { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.Int32Reader";

        public override object Deserialize(XnbContentReader reader)
        {
            return (object)reader.ReadInt32();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            writer.Write((int)data);
        }
    }
}
