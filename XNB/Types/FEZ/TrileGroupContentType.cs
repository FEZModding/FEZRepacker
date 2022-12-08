using FEZEngine;
using FEZEngine.Structure;
using FEZRepacker.Dependencies;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class TrileGroupContentType : XNBContentType<TrileGroup>
    {
        public TrileGroupContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.TrileGroupReader";

        public override object Read(BinaryReader reader)
        {
            TrileGroup trileGroup = new TrileGroup();

            trileGroup.Triles = Converter.ReadType<List<TrileInstance>>(reader) ?? trileGroup.Triles;
            trileGroup.Path = Converter.ReadType<MovementPath>(reader) ?? trileGroup.Path;
            trileGroup.Heavy = reader.ReadBoolean();
            trileGroup.ActorType = Converter.ReadType<ActorType>(reader);
            trileGroup.GeyserOffset = reader.ReadSingle();
            trileGroup.GeyserPauseFor = reader.ReadSingle();
            trileGroup.GeyserLiftFor = reader.ReadSingle();
            trileGroup.GeyserApexHeight = reader.ReadSingle();
            trileGroup.SpinCenter = reader.ReadVector3();
            trileGroup.SpinClockwise = reader.ReadBoolean();
            trileGroup.SpinFrequency = reader.ReadSingle();
            trileGroup.SpinNeedsTriggering = reader.ReadBoolean();
            trileGroup.Spin180Degrees = reader.ReadBoolean();
            trileGroup.FallOnRotate = reader.ReadBoolean();
            trileGroup.SpinOffset = reader.ReadSingle();
            trileGroup.AssociatedSound = Converter.ReadType<string>(reader) ?? trileGroup.AssociatedSound;

            return trileGroup;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileGroup trileGroup = (TrileGroup)data;
            Converter.WriteType(trileGroup.Triles, writer);
            Converter.WriteType(trileGroup.Path, writer);
            writer.Write(trileGroup.Heavy);
            Converter.WriteType(trileGroup.ActorType, writer);
            writer.Write(trileGroup.GeyserOffset);
            writer.Write(trileGroup.GeyserPauseFor);
            writer.Write(trileGroup.GeyserLiftFor);
            writer.Write(trileGroup.GeyserApexHeight);
            writer.Write(trileGroup.SpinCenter);
            writer.Write(trileGroup.SpinClockwise);
            writer.Write(trileGroup.SpinFrequency);
            writer.Write(trileGroup.SpinNeedsTriggering);
            writer.Write(trileGroup.Spin180Degrees);
            writer.Write(trileGroup.FallOnRotate);
            writer.Write(trileGroup.SpinOffset); 
            Converter.WriteType(trileGroup.AssociatedSound, writer);
        }
    }
}
