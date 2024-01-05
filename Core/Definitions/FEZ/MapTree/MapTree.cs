namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.MapTree")]
    [XnbReaderType("FezEngine.Readers.MapTreeReader")]
    internal class MapTree
    {
        [XnbProperty(UseConverter = true)]
        public MapNode Root { get; set; }

        public MapTree()
        {
            Root = new MapNode();
        }
    }
}
