using FEZRepacker.Converter.Definitions.FezEngine;
using FEZRepacker.Converter.Definitions.FezEngine.Structure;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomStructures
{
    internal static class ModifiedMapTree
    {
        public class ModifiedNode
        {
            public string LevelName { get;set; }
            public LevelNodeType NodeType { get;set; }
            public WinConditions Conditions { get; set; }
            public bool HasLesserGate { get; set; }
            public bool HasWarpGate { get; set; }
            public List<ModifiedConnection> Connections { get; set; }
        }

        public class ModifiedConnection
        {
            public FaceOrientation Face { get; set; }
            public int Node { get; set; }
            public float BranchOversize { get; set; }
        }

        public static Dictionary<int, ModifiedNode> FromMapTree(MapTree mapTree)
        {
            var nodesToConvert = new List<MapNode>();
            var nodesToUnpack = new List<MapNode>() { mapTree.Root };

            while (nodesToUnpack.Count > 0)
            {
                nodesToConvert.AddRange(nodesToUnpack);

                nodesToUnpack = nodesToUnpack
                    .SelectMany(node => node.Connections)
                    .Select(conn => conn.Node).ToList();
            }

            return nodesToConvert.Select(node => new ModifiedNode()
            {
                LevelName = node.LevelName,
                NodeType = node.NodeType,
                Conditions = node.Conditions,
                HasLesserGate = node.HasLesserGate,
                HasWarpGate = node.HasWarpGate,
                Connections = node.Connections.Select(conn => new ModifiedConnection()
                {
                    Face = conn.Face,
                    BranchOversize = conn.BranchOversize,
                    Node = nodesToConvert.FindIndex(node => node == conn.Node)
                }).ToList()
            }).Select((node,i) => (Id: i, Node:node))
            .ToDictionary(pair => pair.Id, pair => pair.Node);
        }

        public static MapTree ToMapTree(Dictionary<int, ModifiedNode> mapDict)
        {
            var mapTree = new MapTree();

            var nodesToConvert = new Dictionary<int, MapNode>()
            {
                { 0, mapTree.Root }
            };

            var convertedNodes = new List<int>() { 0 };

            while(nodesToConvert.Count > 0)
            {
                var newNodesToConvert = new Dictionary<int, MapNode>();

                foreach(var nodeRecord in nodesToConvert)
                {
                    var modNode = mapDict[nodeRecord.Key];
                    nodeRecord.Value.LevelName = modNode.LevelName;
                    nodeRecord.Value.NodeType = modNode.NodeType;
                    nodeRecord.Value.Conditions = modNode.Conditions;
                    nodeRecord.Value.HasLesserGate = modNode.HasLesserGate;
                    nodeRecord.Value.HasWarpGate = modNode.HasWarpGate;

                    foreach(var modConnection in modNode.Connections)
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
    }
}
