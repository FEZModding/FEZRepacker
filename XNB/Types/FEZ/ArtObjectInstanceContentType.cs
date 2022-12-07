using System.Numerics;

using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;

namespace FEZRepacker.XNB.Types.FEZ
{
    class ArtObjectInstanceContentType : XNBContentType<ArtObjectInstance>
    {
        public ArtObjectInstanceContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.ArtObjectInstanceReader";

        public override object Read(BinaryReader reader)
        {
            ArtObjectInstance artObject = new ArtObjectInstance();

            artObject.Name = reader.ReadString();

            artObject.Position.X = reader.ReadSingle();
            artObject.Position.Y = reader.ReadSingle();
            artObject.Position.Z = reader.ReadSingle();

            artObject.Rotation.X = reader.ReadSingle();
            artObject.Rotation.Y = reader.ReadSingle();
            artObject.Rotation.Z = reader.ReadSingle();
            artObject.Rotation.W = reader.ReadSingle();

            artObject.Scale.X = reader.ReadSingle();
            artObject.Scale.Y = reader.ReadSingle();
            artObject.Scale.Z = reader.ReadSingle();

            artObject.ActorSettings = Converter.ReadType<ArtObjectActorSettings>(reader) ?? new ArtObjectActorSettings();

            return artObject;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            ArtObjectInstance artObject = (ArtObjectInstance)data;

            writer.Write(artObject.Name);

            writer.Write(artObject.Position.X);
            writer.Write(artObject.Position.Y);
            writer.Write(artObject.Position.Z);

            writer.Write(artObject.Rotation.X);
            writer.Write(artObject.Rotation.Y);
            writer.Write(artObject.Rotation.Z);
            writer.Write(artObject.Rotation.W);

            writer.Write(artObject.Scale.X);
            writer.Write(artObject.Scale.Y);
            writer.Write(artObject.Scale.Z);

            Converter.WriteType(artObject.ActorSettings, writer);
        }
    }
}
