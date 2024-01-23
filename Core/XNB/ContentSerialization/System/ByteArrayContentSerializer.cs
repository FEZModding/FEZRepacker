namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class ByteArrayContentSerializer : XnbContentSerializer<byte[]>
    {
        private XnbAssemblyQualifier _name;
        public ByteArrayContentSerializer() : base()
        {
            _name = typeof(ArrayContentSerializer<byte>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ArrayReader";
            IsPrivate = true;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Deserialize(XnbContentReader reader)
        {
            int dataCount = reader.ReadInt32();
            byte[] data = reader.ReadBytes(dataCount);
            return data;
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            byte[] array = (byte[])data;

            writer.Write(array.Length);
            writer.Write(array);
        }
    }
}
