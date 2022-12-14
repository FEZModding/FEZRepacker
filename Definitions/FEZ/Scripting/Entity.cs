using FEZRepacker.XNB.Attributes;

namespace FEZEngine.Structure.Scripting
{
    [XNBType("FezEngine.Readers.EntityReader")]
    class Entity
    {
        [XNBProperty]
        public string Type { get; set; }

        [XNBProperty(Optional = true)]
        public int? Identifier { get; set; }

        public Entity()
        {
            Type = "";
        }
    }
}
