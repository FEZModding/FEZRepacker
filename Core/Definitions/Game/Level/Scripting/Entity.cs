namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.Entity, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.EntityReader, FezEngine")]
    public class Entity
    {
        [XnbProperty]
        public string Type { get; set; } = "";

        [XnbProperty(Optional = true)]
        public int? Identifier { get; set; }
    }
}
