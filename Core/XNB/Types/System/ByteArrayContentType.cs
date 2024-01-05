using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types.System
{
    internal class ByteArrayContentType : XnbContentType<byte[]>
    {
        private XnbAssemblyQualifier _name;
        public ByteArrayContentType(XnbFormatConverter converter) : base(converter)
        {
            _name = typeof(ArrayContentType<byte>).FullName ?? "";
            _name.Namespace = "Microsoft.Xna.Framework.Content";
            _name.Name = "ArrayReader";
            IsPrivate = true;
        }

        public override XnbAssemblyQualifier Name => _name;

        public override object Read(BinaryReader reader)
        {
            int dataCount = reader.ReadInt32();
            byte[] data = reader.ReadBytes(dataCount);
            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            byte[] array = (byte[])data;

            writer.Write(array.Length);
            writer.Write(array);
        }
    }
}
