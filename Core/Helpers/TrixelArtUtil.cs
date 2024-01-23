using System.Text;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers
{
    internal static class TrixelArtUtil
    {
        public static Dictionary<string, IndexedPrimitives<VertexInstance, VertexType>> LoadGeometry<VertexType>(Stream geometryStream)
        {
            using var geometryReader = new BinaryReader(geometryStream, Encoding.UTF8, true);
            string geometryString = new string(geometryReader.ReadChars((int)geometryStream.Length));
            return WavefrontObjUtil.FromWavefrontObj<VertexType>(geometryString);
        }

        /// <summary>
        /// Recalculates texture coordinates to match what game does with them during loading process.
        /// </summary>
        /// <typeparam name="VertexType">Type of modified geometry</typeparam>
        /// <param name="geometry">Geometry to modify</param>
        /// <param name="geometryBounds">Bounds of the geometry to calculate texture coordinates with.</param>
        public static void RecalculateCubemapTexCoords<VertexType>(IndexedPrimitives<VertexInstance, VertexType> geometry, Vector3 geometryBounds) {
            foreach (var vertex in geometry.Vertices)
            {
                (int textureOffset, Vector3 xAxis, Vector3 yAxis) = vertex.NormalByte switch
                {
                    5 => (0, new Vector3(1, 0, 0), new Vector3(0, -1, 0)), // front
                    3 => (1, new Vector3(0, 0, -1), new Vector3(0, -1, 0)), // right
                    2 => (2, new Vector3(-1, 0, 0), new Vector3(0, -1, 0)), // back
                    0 => (3, new Vector3(0, 0, 1), new Vector3(0, -1, 0)), // left
                    4 => (4, new Vector3(1, 0, 0), new Vector3(0, 0, 1)), // top
                    1 => (5, new Vector3(1, 0, 0), new Vector3(0, 0, -1)), // bottom
                };

                var texturePlanePosition = vertex.Position / geometryBounds;
                vertex.TextureCoordinate = new Vector2(
                    (Vector3.Dot(texturePlanePosition, xAxis) + 0.5f + textureOffset) / 6.0f,
                    Vector3.Dot(texturePlanePosition, yAxis) + 0.5f
                );
            }
        }
    }
}
