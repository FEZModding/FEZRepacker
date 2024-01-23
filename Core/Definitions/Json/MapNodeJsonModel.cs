
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.MapTree;

namespace FEZRepacker.Core.Definitions.Json
{
    internal class MapNodeJsonModel : JsonModel<MapNode>
    {
        public string LevelName { get; set; }
        public LevelNodeType NodeType { get; set; }
        public WinConditions Conditions { get; set; }
        public bool HasLesserGate { get; set; }
        public bool HasWarpGate { get; set; }
        public List<MapNodeConnectionJsonModel> Connections { get; set; }

        public MapNodeJsonModel()
        {
            LevelName = "";
            Conditions = new();
            Connections = new();
        }

        public MapNodeJsonModel(MapNode data) : this()
        {
            SerializeFrom(data);
        }

        public MapNode Deserialize()
        {
            return new()
            {
                LevelName = LevelName,
                NodeType = NodeType,
                Conditions = Conditions,
                HasLesserGate = HasLesserGate,
                HasWarpGate = HasWarpGate
            };
        }

        public void SerializeFrom(MapNode data)
        {
            LevelName = data.LevelName;
            NodeType = data.NodeType;
            Conditions = data.Conditions;
            HasLesserGate = data.HasLesserGate;
            HasWarpGate = data.HasWarpGate;
        }
    }
}
