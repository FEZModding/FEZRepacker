namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.Entity")]
    [XnbReaderType("FezEngine.Readers.EntityReader")]
    class Entity
    {
        [XnbProperty]
        public string Type { get; set; }

        [XnbProperty(Optional = true)]
        public int? Identifier { get; set; }

        public Entity()
        {
            Type = "";
        }
    }
}