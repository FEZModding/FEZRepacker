using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class TrileEmplacementContentType : XNBContentType<TrileEmplacement>
    {
        public TrileEmplacementContentType(XNBContentConverter converter) : base(converter) { }

        public override TypeAssemblyQualifier Name => "FezEngine.Readers.TrileEmplacementReader";

        public override object Read(BinaryReader reader)
        {
            TrileEmplacement trilePos = new TrileEmplacement(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32()
            );

            return trilePos;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileEmplacement trilePos = (TrileEmplacement)data;
            writer.Write(trilePos.X);
            writer.Write(trilePos.Y);
            writer.Write(trilePos.Z);
        }
    }
}
