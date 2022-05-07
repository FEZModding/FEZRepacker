namespace FEZRepacker.XNB.Types.System
{
    class EnumContentType<T> : XNBContentType<T> where T : Enum
    {
        private TypeAssemblyQualifier _name;

        public EnumContentType(XNBContentConverter converter) : base(converter)
        {
            // creating type assembly qualifier name
            _name = typeof(EnumContentType<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "EnumReader";
        }

        public override TypeAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            int value = (int)data;
            writer.Write(value);
        }
    }
}
