using FEZEngine.Structure.Scripting;

namespace FEZRepacker.XNB.Types.FEZ
{
    class EntityContentType : XNBContentType<Entity>
    {
        public EntityContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "FezEngine.Readers.EntityReader";

        public override object Read(BinaryReader reader)
        {
            Entity entity = new Entity();

            entity.Type = reader.ReadString();
            if (reader.ReadBoolean())
            {
                entity.Identifier = reader.ReadInt32();
            }

            return entity;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Entity entity = (Entity)data;

            writer.Write(entity.Type);
            if (entity.Identifier.HasValue)
            {
                writer.Write(true);
                writer.Write(entity.Identifier.Value);
            }
            else
            {
                writer.Write(false);
            }

        }
    }
}