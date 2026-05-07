using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.MapNode+Connection, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.MapNodeConnectionReader, FezEngine")]
    public class MapNodeConnection
    {
        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }

        [XnbProperty(UseConverter = true)]
        public MapNode Node { get; set; } = new();

        [XnbProperty]
        public float BranchOversize { get;set; }
    }
}
