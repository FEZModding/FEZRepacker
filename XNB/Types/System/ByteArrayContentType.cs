namespace FEZRepacker.XNB.Types.System
{
    class ByteArrayContentType : XNBContentType<byte[]>
    {
        private FEZAssemblyQualifier _name;
        private bool _skipElementType;
        public ByteArrayContentType(XNBContentConverter converter) : base(converter)
        {
            _name = typeof(ArrayContentType<byte>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ArrayReader";
        }

        public override FEZAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            int dataCount = reader.ReadInt32();
            byte[] data = reader.ReadBytes(dataCount);
            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            byte[] array = (byte[])data;

            writer.Write(array.Length);
            writer.Write(array);
        }
    }
}
