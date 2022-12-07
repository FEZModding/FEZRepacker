using FEZEngine;
using FEZEngine.Structure;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class TrileInstanceContentType : XNBContentType<TrileInstance>
    {
        public TrileInstanceContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.TrileInstanceReader";

        public override object Read(BinaryReader reader)
        {
            TrileInstance trile = new TrileInstance();

            trile.Position = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            trile.TrileId = reader.ReadInt32();
            trile.PhiLight = reader.ReadByte();
            if (reader.ReadBoolean())
            {
                trile.ActorSettings = Converter.ReadType<InstanceActorSettings>(reader);
            }
            trile.OverlappedTriples = Converter.ReadType<List<TrileInstance>>(reader) ?? new List<TrileInstance>();

            return trile;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileInstance trile = (TrileInstance)data;
            writer.Write(trile.Position.X);
            writer.Write(trile.Position.Y);
            writer.Write(trile.Position.Z);
            writer.Write(trile.TrileId);
            writer.Write(trile.PhiLight);

            writer.Write(trile.ActorSettings != null);
            if(trile.ActorSettings != null) Converter.WriteType(trile.ActorSettings, writer);

            Converter.WriteType(trile.OverlappedTriples, writer);
        }
    }
}
