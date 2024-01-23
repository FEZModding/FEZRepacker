using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.MapNode")]
    [XnbReaderType("FezEngine.Readers.MapNodeReader")]
    public class MapNode
    {
        [XnbProperty]
        public string LevelName { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<MapNodeConnection> Connections { get; set; }

        [XnbProperty(UseConverter = true)]
        public LevelNodeType NodeType { get; set; }

        [XnbProperty(UseConverter = true)]
        public WinConditions Conditions { get; set; }

        [XnbProperty]
        public bool HasLesserGate { get; set; }

        [XnbProperty]
        public bool HasWarpGate { get; set; }


        public MapNode()
        {
            Connections = new List<MapNodeConnection>();
            Conditions = new WinConditions();
        }
    }
}
