using System.Text;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers
{
    internal static class WavefrontObjUtil
    {
        public static string ToWavefrontObj<T>(Dictionary<string, ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, T>> geometries)
        {
            int indicesOffset = 0;
            var objBuilder = new StringBuilder();
            foreach(var geometry in geometries)
            {
                objBuilder.AppendLine($"o {geometry.Key}");
                objBuilder.AppendLine(geometry.Value.ToWavefrontObj(indicesOffset));
                indicesOffset += geometry.Value.Vertices.Count();
            }
            return objBuilder.ToString();
        }

        public static string ToWavefrontObj<T>(this ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance,T> geometry, int indicesOffset = 0)
        {
            var objBuilder = new StringBuilder();

            var vertices = new List<Vector3>();
            var textureCoordinates = new List<Vector2>();
            var normals = new List<Vector3>();

            foreach (var vertexData in geometry.Vertices)
            {
                vertices.Add(vertexData.Position);
                textureCoordinates.Add(vertexData.TextureCoordinate);
                normals.Add(vertexData.Normal);
            }

            foreach (var vertex in vertices)
            {
                objBuilder.AppendLine($"v {vertex.X} {vertex.Y} {vertex.Z}");
            }

            foreach (var texCoord in textureCoordinates)
            {
                // y coords are inverted
                objBuilder.AppendLine($"vt {texCoord.X} {1.0f - texCoord.Y}");
            }

            foreach (var normal in normals)
            {
                objBuilder.AppendLine($"vn {normal.X} {normal.Y} {normal.Z}");
            }

            var indices = geometry.Indices;
            var type = geometry.PrimitiveType;

            bool isLine = (type == PrimitiveType.LineList || type == PrimitiveType.LineStrip);
            bool isList = (type == PrimitiveType.TriangleList || type == PrimitiveType.LineList);

            int iOffset = indicesOffset + 1; // indexing starts at 1. add given offset on top of that.
            if (isLine)
            {
                for (int i = 1; i < indices.Count() - indices.Count() % 2; i += (isList ? 2 : 1))
                {
                    objBuilder.AppendLine($"l {indices[i - 1] + iOffset} {indices[i] + iOffset}");
                }
            }
            else
            {
                for (int i = 2; i < indices.Count() - indices.Count() % 3; i += (isList ? 3 : 1))
                {
                    // revert orders of indices
                    int i3 = indices[i - 2] + iOffset;
                    int i2 = indices[i - 1] + iOffset;
                    int i1 = indices[i] + iOffset;

                    objBuilder.AppendLine($"f {i1}/{i1}/{i1} {i2}/{i2}/{i2} {i3}/{i3}/{i3}");
                }
            }

            return objBuilder.ToString();
        }

        public static Dictionary<string, ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, T>> FromWavefrontObj<T>(string obj)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();

            var objectNames = new List<string> { "UNNAMED" };
            var indicesGroup = new List<List<string>> { new List<string>() };

            using var objReader = new StringReader(obj);
            for (string line = objReader.ReadLine(); line != null; line = objReader.ReadLine())
            {
                var tokens = line.Split(new char[] {' '});
                if (tokens.Length < 2) continue;

                if(tokens[0] == "o")
                {
                    var newObjectName = tokens.Length > 1 ? tokens[1] : "UNNAMED";

                    // make sure there are no repeats in names
                    var indexedNewObjectName = newObjectName;
                    var index = 2;
                    while (objectNames.Contains(indexedNewObjectName))
                    {
                        indexedNewObjectName = $"{newObjectName}_{index}";
                        index++;
                    }

                    if(indicesGroup[indicesGroup.Count - 1].Count > 0)
                    {
                        indicesGroup.Add(new List<string>());
                        objectNames.Add(indexedNewObjectName);
                    }
                    else
                    {
                        objectNames[objectNames.Count - 1] = indexedNewObjectName;
                    }
                }

                else if(tokens[0] == "v" && tokens.Length >= 4)
                {
                    var x = float.Parse(tokens[1]);
                    var y = float.Parse(tokens[2]);
                    var z = float.Parse(tokens[3]);
                    vertices.Add(new Vector3(x, y, z));
                }
                else if(tokens[0] == "vt" && tokens.Length >= 3)
                {
                    var x = float.Parse(tokens[1]);
                    var y = float.Parse(tokens[2]);
                    uvs.Add(new Vector2(x, 1.0f - y)); // y is inverted
                }
                else if(tokens[0] == "vn" && tokens.Length >= 4)
                {
                    var x = float.Parse(tokens[1]);
                    var y = float.Parse(tokens[2]);
                    var z = float.Parse(tokens[3]);
                    normals.Add(new Vector3(x, y, z));
                }

                else if(tokens[0] == "f" && tokens.Length >= 4)
                {
                    // revert orders of indices
                    indicesGroup[indicesGroup.Count - 1].Add(tokens[3]);
                    indicesGroup[indicesGroup.Count - 1].Add(tokens[2]);
                    indicesGroup[indicesGroup.Count - 1].Add(tokens[1]);
                }
            }

            var geometryList = new Dictionary<string, ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, T>>();

            for (int i = 0; i < indicesGroup.Count; i++)
            {
                var stringVerticesList = new List<string>();
                var indicesList = new List<ushort>();
                foreach (var indexString in indicesGroup[i]) {
                    var vertexIndex = stringVerticesList.IndexOf(indexString);
                    if(vertexIndex >= 0)
                    {
                        indicesList.Add((ushort)vertexIndex);
                    }
                    else
                    {
                        indicesList.Add((ushort)stringVerticesList.Count);
                        stringVerticesList.Add(indexString);
                    }
                }

                var verticesList = new List<VertexPositionNormalTextureInstance>();
                foreach (var indexString in stringVerticesList)
                {
                    var splitIndex = indexString.Split(new char[] { '/' });

                    var vertexIndex = splitIndex.Length > 0 ? int.Parse(splitIndex[0]) : 0;
                    var uvIndex = splitIndex.Length > 1 ? int.Parse(splitIndex[1]) : 0;
                    var normalIndex = splitIndex.Length > 2 ? int.Parse(splitIndex[2]) : 0;

                    var vertex = new VertexPositionNormalTextureInstance();
                    if (vertexIndex > 0 && vertexIndex <= vertices.Count) vertex.Position = vertices[vertexIndex-1];
                    if (uvIndex > 0 && uvIndex <= uvs.Count) vertex.TextureCoordinate = uvs[uvIndex-1];
                    if (normalIndex > 0 && normalIndex <= normals.Count) vertex.Normal = normals[normalIndex-1];

                    verticesList.Add(vertex);
                }

                var geometry = new ShaderInstancedIndexedPrimitives<VertexPositionNormalTextureInstance, T>();
                geometry.PrimitiveType = PrimitiveType.TriangleList;
                geometry.Vertices = verticesList.ToArray();
                geometry.Indices = indicesList.ToArray();

                geometryList[objectNames[i]] = geometry;
            }

            return geometryList;
        }
    }
}
