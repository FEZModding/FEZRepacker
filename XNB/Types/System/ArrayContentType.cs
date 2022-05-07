namespace FEZRepacker.XNB.Types.System
{
    class ArrayContentType<T> : XNBContentType<T[]> where T : notnull
    {
        private TypeAssemblyQualifier _name;
        public ArrayContentType(XNBContentConverter converter) : base(converter)
        {
            // creating type assembly qualifier name
            _name = typeof(ArrayContentType<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ArrayReader";
        }

        public override TypeAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            int dataCount = reader.ReadInt32();
            T[] data = new T[dataCount];
            for (int i = 0; i < dataCount; i++)
            {
                T? value = Converter.ReadType<T>(reader, true);
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
                Converter.WriteType<T>(value, writer, true);
            }
        }
    }
}
