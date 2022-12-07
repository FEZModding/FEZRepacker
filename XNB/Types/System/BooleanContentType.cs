namespace FEZRepacker.XNB.Types.System
{
    class BooleanContentType : XNBContentType<bool>
    {
        public BooleanContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.BooleanReader";

        public override object Read(BinaryReader reader)
        {
            return (object)reader.ReadBoolean();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((bool)data);
        }
    }
}
