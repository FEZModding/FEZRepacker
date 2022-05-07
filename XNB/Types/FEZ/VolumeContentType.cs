using FEZEngine;
using FEZEngine.Structure;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class VolumeContentType : XNBContentType<Volume>
    {
        public VolumeContentType(XNBContentConverter converter) : base(converter) { }

        public override TypeAssemblyQualifier Name => "FezEngine.Readers.LevelReader";

        public override object Read(BinaryReader reader)
        {
            Volume volume = new Volume();

            volume.Orientations = Converter.ReadType<FaceOrientation[]>(reader);
            volume.From = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            volume.To = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            volume.ActorSettings = Converter.ReadType<VolumeActorSettings>(reader);

            return volume;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Volume volume = (Volume)data;
            Converter.WriteType(volume.Orientations, writer);
            writer.Write(volume.From.X);
            writer.Write(volume.From.Y);
            writer.Write(volume.From.Z);
            writer.Write(volume.To.X);
            writer.Write(volume.To.Y);
            writer.Write(volume.To.Z);
            Converter.WriteType(volume.ActorSettings, writer);
        }
    }
}
