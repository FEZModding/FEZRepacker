using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class DictionaryContentType<K,V> : XnbContentType<Dictionary<K,V>> where K : notnull
    {
        private XnbAssemblyQualifier _name;
        private bool skipKeyIdentifier;
        private bool skipValueIdentifier;

        public DictionaryContentType(
            XnbFormatConverter converter, 
            bool skipKeyIdentifier = false, 
            bool skipValueIdentifier = false
        ) : base(converter){
            // creating type assembly qualifier name, since we're using own types
            _name = BasicType.FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "DictionaryReader";

            var genericKeyQualifier = GetXnbTypeFor(typeof(K));
            if (genericKeyQualifier.HasValue) _name.Templates[0] = genericKeyQualifier.Value;

            var genericValueQualifier = GetXnbTypeFor(typeof(V));
            if (genericValueQualifier.HasValue) _name.Templates[1] = genericValueQualifier.Value;

            this.skipKeyIdentifier = skipKeyIdentifier;
            this.skipValueIdentifier = skipValueIdentifier;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            Dictionary<K, V> data = new Dictionary<K, V>();
            int dataCount = reader.ReadInt32();
            for(int i = 0; i < dataCount; i++)
            {
                K? key = Converter.ReadType<K>(reader, skipKeyIdentifier);
                V? value = Converter.ReadType<V>(reader, skipValueIdentifier);
                if(key != null && value != null) data[key] = value;
            }
            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Dictionary<K, V> dict = (Dictionary<K, V>)data;

            writer.Write(dict.Count);
            foreach(var record in dict)
            {
                Converter.WriteType<K>(record.Key, writer, skipKeyIdentifier);
                Converter.WriteType<V>(record.Value, writer, skipValueIdentifier);
            }
        }
    }
}
