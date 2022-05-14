namespace FEZRepacker.XNB.Types.System
{
    class ListContentType<T> : XNBContentType<List<T>> where T : notnull
    {
        private FEZAssemblyQualifier _name;
        public ListContentType(XNBContentConverter converter) : base(converter)
        {
            // creating type assembly qualifier name
            _name = typeof(ArrayContentType<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ListReader";
        }

        public override FEZAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            List<T> data = new List<T>();
            int dataCount = reader.ReadInt32();
            for (int i = 0; i < dataCount; i++)
            {
                T? value = Converter.ReadType<T>(reader);
                if (value != null) data.Add(value);
            }
            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            List<T> list = (List<T>)data;

            writer.Write(list.Count);
            foreach (T value in list)
            {
                Converter.WriteType<T>(value, writer);
            }
        }
    }
}
