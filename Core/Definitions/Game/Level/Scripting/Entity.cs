namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.Entity")]
    [XnbReaderType("FezEngine.Readers.EntityReader")]
    public class Entity
    {
        [XnbProperty]
        public string Type { get; set; } = "";

        [XnbProperty(Optional = true)]
        public int? Identifier { get; set; }
    }
}
