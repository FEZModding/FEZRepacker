
namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class ArrayContentSerializer<T> : XnbContentSerializer<T[]> where T : notnull
    {
        private XnbAssemblyQualifier _name;
        private readonly bool _skipElementType;
        public ArrayContentSerializer(bool skipElementType = true) : base()
        {
            // creating type assembly qualifier name
            _name = typeof(ArrayContentSerializer<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ArrayReader";

            var genericQualifier = XnbAssemblyQualifier.GetFromXnbType(typeof(T));
            if (genericQualifier.HasValue) _name.Templates[0] = genericQualifier.Value;

            // some arrays have element type prefixes, some dont.
            // i have no idea what's the rule here, im just making it a variable
            _skipElementType = skipElementType;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Deserialize(XnbContentReader reader)
        {
            int dataCount = reader.ReadInt32();
            T[] data = new T[dataCount];
            for (int i = 0; i < dataCount; i++)
            {
                T? value = reader.ReadContent<T>(_skipElementType);
                if (value != null) data[i] = value;
            }
            return data;
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            T[] array = (T[])data;

            writer.Write(array.Length);
            foreach (T value in array)
            {
                writer.WriteContent<T>(value, _skipElementType);
            }
        }
    }
}
