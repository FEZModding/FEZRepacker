namespace FEZRepacker.XNB.Types.System
{
    class StringContentType : XNBContentType<string>
    {
        public StringContentType(XNBContentConverter converter) : base(converter){}
        public override FEZAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.StringReader";

        public override object Read(BinaryReader reader)
        {
            return reader.ReadString();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((string)data);
        }

        public override bool IsEmpty(object data)
        {
            return ((string)data).Length == 0;
        }
    }
}
