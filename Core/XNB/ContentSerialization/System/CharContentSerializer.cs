namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class CharContentSerializer : XnbContentSerializer<char>
    {
        public CharContentSerializer() : base() { }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.CharReader";

        public override object Deserialize(XnbContentReader reader)
        {
            return (object)reader.ReadChar();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            writer.Write((char)data);
        }
    }
}
