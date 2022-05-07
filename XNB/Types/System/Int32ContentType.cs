namespace FEZRepacker.XNB.Types.System
{
    class Int32ContentType : XNBContentType<int>
    {
        public Int32ContentType(XNBContentConverter converter) : base(converter) { }
        public override TypeAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.Int32Reader";

        public override object Read(BinaryReader reader)
        {
            return (object)reader.ReadInt32();
        }

        public override void Write(object data, BinaryWriter writer)
        {
            writer.Write((int)data);
        }
    }
}
