
using FEZRepacker.Core.Definitions.Game.MapTree;

namespace FEZRepacker.Core.Definitions.Json
{
    public class MapTreeJsonModel : Dictionary<int, MapNodeJsonModel>, JsonModel<MapTree>
    {
        public MapTree Deserialize()
        {
            if (Count == 0)
            {
                return new MapTree();
            }

            var mapTree = new MapTree
            {
                Root = new MapNode()
            };

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
                        var connection = new MapNodeConnection()
                        {
                            Face = modConnection.Face,
                            BranchOversize = modConnection.BranchOversize
                        };

                        if (modConnection.Node is { } nodeIndex)
                        {
                            if (convertedNodes.Contains(nodeIndex)) continue;
                            convertedNodes.Add(nodeRecord.Key);

                            var connectedNode = new MapNode();
                            connection.Node = connectedNode;
                            newNodesToConvert[nodeIndex] = connectedNode;
                        }

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
                    Node = FindNodeIndex(nodesToConvert, conn.Node)
                }).ToList()
            }).Select((node, index) => (Node: node, Index: index));

            foreach ((var node, var index) in convertedIndexedNodes) this[index] = node;
        }

        private static int? FindNodeIndex(List<MapNode> nodes, MapNode? node)
        {
            if (node == null) return null;
            return nodes.FindIndex(other => other == node);
        }

        private static List<MapNode> UnpackMapNodes(MapNode? rootNode)
        {
            if (rootNode == null)
            {
                return new List<MapNode>();
            }
            
            var nodesToConvert = new List<MapNode>();
            var nodesToUnpack = new List<MapNode>() { rootNode };

            while (nodesToUnpack.Count > 0)
            {
                nodesToConvert.AddRange(nodesToUnpack);

                nodesToUnpack = nodesToUnpack
                    .SelectMany(node => node.Connections)
                    .Select(conn => conn.Node)
                    .OfType<MapNode>().ToList();
            }

            return nodesToConvert;
        }
    }
}
