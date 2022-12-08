using FEZEngine;
using FEZEngine.Structure;
using FEZRepacker.Dependencies;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class VolumeContentType : XNBContentType<Volume>
    {
        public VolumeContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.VolumeReader";

        public override object Read(BinaryReader reader)
        {
            Volume volume = new Volume();

            volume.Orientations = Converter.ReadType<FaceOrientation[]>(reader) ?? volume.Orientations;
            volume.From = reader.ReadVector3();
            volume.To = reader.ReadVector3();
            volume.ActorSettings = Converter.ReadType<VolumeActorSettings>(reader) ?? volume.ActorSettings;

            return volume;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Volume volume = (Volume)data;
            Converter.WriteType(volume.Orientations, writer);
            writer.Write(volume.From);
            writer.Write(volume.To);
            Converter.WriteType(volume.ActorSettings, writer);
        }
    }
}
