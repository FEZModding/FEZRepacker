using FEZEngine.Structure;
using FEZRepacker.Dependencies;

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

            artObject.Position = reader.ReadVector3();
            artObject.Rotation = reader.ReadQuaternion();
            artObject.Scale = reader.ReadVector3();

            artObject.ActorSettings = Converter.ReadType<ArtObjectActorSettings>(reader) ?? artObject.ActorSettings;

            return artObject;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            ArtObjectInstance artObject = (ArtObjectInstance)data;

            writer.Write(artObject.Name);

            writer.Write(artObject.Position);
            writer.Write(artObject.Rotation);
            writer.Write(artObject.Scale);

            Converter.WriteType(artObject.ActorSettings, writer);
        }
    }
}
