namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class BooleanContentSerializer : XnbContentSerializer<bool>
    {
        public BooleanContentSerializer() : base() { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.BooleanReader";

        public override object Deserialize(XnbContentReader reader)
        {
            return (object)reader.ReadBoolean();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            writer.Write((bool)data);
        }
    }
}
