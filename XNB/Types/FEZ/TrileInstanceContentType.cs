using FEZEngine;
using FEZEngine.Structure;
using FEZRepacker.Dependencies;
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

            trile.Position = reader.ReadVector3();
            trile.TrileId = reader.ReadInt32();
            trile.PhiLight = reader.ReadByte();
            if (reader.ReadBoolean()) trile.ActorSettings = Converter.ReadType<TrileInstanceActorSettings>(reader);
            trile.OverlappedTriples = Converter.ReadType<List<TrileInstance>>(reader) ?? new List<TrileInstance>();

            return trile;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileInstance trile = (TrileInstance)data;
            writer.Write(trile.Position);
            writer.Write(trile.TrileId);
            writer.Write(trile.PhiLight);

            writer.Write(trile.ActorSettings != null);
            if(trile.ActorSettings != null) Converter.WriteType(trile.ActorSettings, writer);

            Converter.WriteType(trile.OverlappedTriples, writer);
        }
    }
}
