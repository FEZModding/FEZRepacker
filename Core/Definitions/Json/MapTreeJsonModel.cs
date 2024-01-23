
using FEZRepacker.Core.Definitions.Game.MapTree;

namespace FEZRepacker.Core.Definitions.Json
{
    public class MapTreeJsonModel : Dictionary<int, MapNodeJsonModel>, JsonModel<MapTree>
    {
        public MapTree Deserialize()
        {
            var mapTree = new MapTree();

            var nodesToConvert = new Dictionary<int, MapNode>()
            {
                { 0, mapTree.Root }
            };

            var convertedNodes = new List<int>() { 0 };

            while (nodesToConvert.Count > 0)
            {
                var newNodesToConvert = new Dictionary<int, MapNode>();

                foreach (var nodeRecord in nodesToConvert)
                {
                    var modNode = this[nodeRecord.Key];
                    nodeRecord.Value.LevelName = modNode.LevelName;
                    nodeRecord.Value.NodeType = modNode.NodeType;
                    nodeRecord.Value.Conditions = modNode.Conditions;
                    nodeRecord.Value.HasLesserGate = modNode.HasLesserGate;
                    nodeRecord.Value.HasWarpGate = modNode.HasWarpGate;

                    foreach (var modConnection in modNode.Connections)
                    {
                        if (convertedNodes.Contains(modConnection.Node)) continue;
                        convertedNodes.Add(nodeRecord.Key);

                        var connection = new MapNodeConnection()
                        {
                            Face = modConnection.Face,
                            BranchOversize = modConnection.BranchOversize,
                            Node = new MapNode()
                        };

                        newNodesToConvert[modConnection.Node] = connection.Node;
                        nodeRecord.Value.Connections.Add(connection);
                    }
                }

                nodesToConvert = newNodesToConvert;
            }

            return mapTree;
        }

        public void SerializeFrom(MapTree data)
        {
            var nodesToConvert = UnpackMapNodes(data.Root);

            var convertedIndexedNodes = nodesToConvert.Select(node => new MapNodeJsonModel(node)
            {
                Connections = node.Connections.Select(conn => new MapNodeConnectionJsonModel(conn)
                {
                    Node = nodesToConvert.FindIndex(node => node == conn.Node)
                }).ToList()
            }).Select((node, index) => (Node: node, Index: index));

            foreach ((var node, var index) in convertedIndexedNodes) this[index] = node;
        }

        private List<MapNode> UnpackMapNodes(MapNode rootNode)
        {
            var nodesToConvert = new List<MapNode>();
            var nodesToUnpack = new List<MapNode>() { rootNode };

            while (nodesToUnpack.Count > 0)
            {
                nodesToConvert.AddRange(nodesToUnpack);

                nodesToUnpack = nodesToUnpack
                    .SelectMany(node => node.Connections)
                    .Select(conn => conn.Node).ToList();
            }

            return nodesToConvert;
        }
    }
}
