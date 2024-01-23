namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class DictionaryContentSerializer<K, V> : XnbContentSerializer<Dictionary<K, V>> where K : notnull
    {
        private XnbAssemblyQualifier _name;
        private bool skipKeyIdentifier;
        private bool skipValueIdentifier;

        public DictionaryContentSerializer(
            bool skipKeyIdentifier = false,
            bool skipValueIdentifier = false
        ) : base()
        {
            // creating type assembly qualifier name, since we're using own types
            _name = ContentType.FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "DictionaryReader";

            var genericKeyQualifier = XnbAssemblyQualifier.GetFromXnbType(typeof(K));
            if (genericKeyQualifier.HasValue) _name.Templates[0] = genericKeyQualifier.Value;

            var genericValueQualifier = XnbAssemblyQualifier.GetFromXnbType(typeof(V));
            if (genericValueQualifier.HasValue) _name.Templates[1] = genericValueQualifier.Value;

            this.skipKeyIdentifier = skipKeyIdentifier;
            this.skipValueIdentifier = skipValueIdentifier;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Deserialize(XnbContentReader reader)
        {
            Dictionary<K, V> data = new Dictionary<K, V>();
            int dataCount = reader.ReadInt32();
            for (int i = 0; i < dataCount; i++)
            {
                K? key = reader.ReadContent<K>(skipKeyIdentifier);
                V? value = reader.ReadContent<V>(skipValueIdentifier);
                if (key != null && value != null) data[key] = value;
            }
            return data;
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            Dictionary<K, V> dict = (Dictionary<K, V>)data;

            writer.Write(dict.Count);
            foreach (var record in dict)
            {
                writer.WriteContent<K>(record.Key, skipKeyIdentifier);
                writer.WriteContent<V>(record.Value, skipValueIdentifier);
            }
        }
    }
}
