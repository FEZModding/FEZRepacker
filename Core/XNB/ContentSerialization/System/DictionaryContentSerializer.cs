using FEZRepacker.Core.Helpers;

namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class DictionaryContentSerializer<K, V> : XnbContentSerializer<OrderedDictionary<K, V>> where K : notnull
    {
        private XnbAssemblyQualifier _name;
        private bool skipKeyIdentifier;
        private bool skipValueIdentifier;

        public override Type ContentType => typeof(IDictionary<K, V>);
        public override XnbAssemblyQualifier Name => _name;
        
        public DictionaryContentSerializer(
            bool skipKeyIdentifier = false,
            bool skipValueIdentifier = false
        ) : base()
        {
            // creating type assembly qualifier name, since we're using own types
            _name = ContentType.FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "DictionaryReader";

            _name.Templates[0] = XnbAssemblyQualifier.GetForType(typeof(K));
            _name.Templates[1] = XnbAssemblyQualifier.GetForType(typeof(V));

            this.skipKeyIdentifier = skipKeyIdentifier;
            this.skipValueIdentifier = skipValueIdentifier;
        }

        public override object Deserialize(XnbContentReader reader)
        {
            var data = new OrderedDictionary<K, V>();
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
            var dict = (IDictionary<K, V>)data;

            writer.Write(dict.Count);
            foreach (var record in dict)
            {
                writer.WriteContent(record.Key, skipKeyIdentifier);
                writer.WriteContent(record.Value, skipValueIdentifier);
            }
        }
    }
}
