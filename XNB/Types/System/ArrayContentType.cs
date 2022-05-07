namespace FEZRepacker.XNB.Types.System
{
    class ArrayContentType<T> : XNBContentType<T[]> where T : notnull
    {
        private TypeAssemblyQualifier _name;
        private bool _skipElementType;
        public ArrayContentType(XNBContentConverter converter, bool skipElementType = true) : base(converter)
        {
            // creating type assembly qualifier name
            _name = typeof(ArrayContentType<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ArrayReader";

            // some arrays have element type prefixes, some dont.
            // i have no idea what's the rule here, im just making it a variable
            _skipElementType = skipElementType;
        }

        public override TypeAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            int dataCount = reader.ReadInt32();
            T[] data = new T[dataCount];
            for (int i = 0; i < dataCount; i++)
            {
                T? value = Converter.ReadType<T>(reader, _skipElementType);
                if (value != null) data[i] = value;
            }
            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            T[] array = (T[])data;

            writer.Write(array.Length);
            foreach (T value in array)
            {
                Converter.WriteType<T>(value, writer, _skipElementType);
            }
        }
    }
}
