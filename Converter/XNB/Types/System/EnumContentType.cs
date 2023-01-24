using FEZRepacker.Converter.Definitions;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class EnumContentType<T> : XnbContentType<T> where T : Enum
    {
        private XnbAssemblyQualifier _name;

        public EnumContentType(XnbFormatConverter converter) : base(converter)
        {
            // creating type assembly qualifier name
            _name = typeof(EnumContentType<T>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "EnumReader";

            var attributes = typeof(T).GetCustomAttributes(typeof(XnbEnumTypeAttribute), false);
            if (attributes.Length > 0)
            {
                _name.Templates[0] = (attributes.First() as XnbEnumTypeAttribute)!.Qualifier;
            }
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            int value = (int)data;
            writer.Write(value);
        }
    }
}
