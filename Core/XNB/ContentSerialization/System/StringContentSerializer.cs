namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class StringContentSerializer : XnbContentSerializer<string>
    {
        private bool isNullable;

        public StringContentSerializer(bool nullable = true) : base() {
            isNullable = nullable;
        }
        public override XnbAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.StringReader";


        public override object Deserialize(XnbContentReader reader)
        {
            return reader.ReadString();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            writer.Write((string)data);
        }

        public override bool IsEmpty(object data)
        {
            return isNullable && ((string)data).Length == 0;
        }
    }
}
