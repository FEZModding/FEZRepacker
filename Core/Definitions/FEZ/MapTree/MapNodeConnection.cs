namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.MapNode+Connection")]
    [XnbReaderType("FezEngine.Readers.MapNodeConnectionReader")]
    internal class MapNodeConnection
    {
        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }

        [XnbProperty(UseConverter = true)]
        public MapNode Node { get; set; }

        [XnbProperty]
        public float BranchOversize { get;set; }


        public MapNodeConnection()
        {
            Node = new MapNode();
        }
    }
}
