namespace FEZRepacker.Core.XNB.ContentSerialization.System
{
    internal class EnumContentSerializer<T> : XnbContentSerializer<T> where T : Enum
    {
        private XnbAssemblyQualifier _name;

        public EnumContentSerializer() : base()
        {
            // creating type assembly qualifier name
            _name = typeof(EnumContentSerializer<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "EnumReader";

            var enumQualifier = XnbAssemblyQualifier.GetFromXnbType(typeof(T));
            if (enumQualifier.HasValue) _name.Templates[0] = enumQualifier.Value;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Deserialize(XnbContentReader reader)
        {
            return reader.ReadInt32();
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            int value = (int)data;
            writer.Write(value);
        }
    }
}
