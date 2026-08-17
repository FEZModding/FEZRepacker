namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.MapTree, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.MapTreeReader, FezEngine")]
    public class MapTree
    {
        [XnbProperty(UseConverter = true)]
        public MapNode Root { get; set; } = new();
    }
}
