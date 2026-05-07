using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.MapNode, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.MapNodeReader, FezEngine")]
    public class MapNode
    {
        [XnbProperty]
        public string LevelName { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public List<MapNodeConnection> Connections { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public LevelNodeType NodeType { get; set; }

        [XnbProperty(UseConverter = true)]
        public WinConditions Conditions { get; set; } = new();

        [XnbProperty]
        public bool HasLesserGate { get; set; }

        [XnbProperty]
        public bool HasWarpGate { get; set; }
    }
}
