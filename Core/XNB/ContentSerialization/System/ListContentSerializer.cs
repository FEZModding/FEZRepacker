namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class ListContentSerializer<T> : XnbContentSerializer<List<T>> where T : notnull
    {
        private XnbAssemblyQualifier _name;
        private bool _skipElementType;
        public ListContentSerializer(bool skipElementType = false) : base()
        {
            // creating type assembly qualifier name
            _name = typeof(ArrayContentSerializer<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ListReader";

            var genericQualifier = XnbAssemblyQualifier.GetFromXnbType(typeof(T));
            if (genericQualifier.HasValue) _name.Templates[0] = genericQualifier.Value;

            // similarly to arrays, elements can have type identifier prefix
            // but unlike arrays, this is much less common
            // again, barely any idea what's the rule here.
            _skipElementType = skipElementType;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Deserialize(XnbContentReader reader)
        {
            List<T> data = new List<T>();
            int dataCount = reader.ReadInt32();
            for (int i = 0; i < dataCount; i++)
            {
                T? value = reader.ReadContent<T>(_skipElementType);
                if (value != null) data.Add(value);
            }
            return data;
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            List<T> list = (List<T>)data;

            writer.Write(list.Count);
            foreach (T value in list)
            {
                writer.WriteContent<T>(value, _skipElementType);
            }
        }

        public override bool IsEmpty(object data)
        {
            return ((List<T>)data).Count == 0;
        }
    }
}
