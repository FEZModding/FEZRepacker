namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class EnumContentSerializer<T> : XnbContentSerializer<T> where T : Enum
    {
        private XnbAssemblyQualifier _name;
        
        public override XnbAssemblyQualifier Name => _name;
        public override Type[] UnderlyingContentTypes => [typeof(int)];
        
        public EnumContentSerializer() : base()
        {
            // creating type assembly qualifier name
            _name = typeof(EnumContentSerializer<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "EnumReader";

            _name.Templates[0] = XnbAssemblyQualifier.GetForType(typeof(T));
        }
        

        public override object Deserialize(XnbContentReader reader)
        {
            return reader.ReadInt32();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            int value = (int)data;
            
            if (!IsPrivate)
            {
                // to ensure underlying type serializer claim, we're writing integer value as content
                // but only when private to prevent internal helper serializers from being claimed.
                writer.WriteContent(value, true);
                return;
            }
            
            writer.Write(value);
        }
    }
}
