using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers
{
    internal static class FezGeometryUtil
    {
        /// Recalculates texture coordinates to match what game does with them during loading process.
        public static void RecalculateCubemapTexCoords<VertexType>(
            IndexedPrimitives<VertexInstance, VertexType> geometry, Vector3 geometryBounds
        ) {
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
                    _ => (0, new Vector3(), new Vector3())
                };

                var texturePlanePosition = vertex.Position / geometryBounds;
                vertex.TextureCoordinate = new Vector2(
                    (Vector3.Dot(texturePlanePosition, xAxis) + 0.5f + textureOffset) / 6.0f,
                    Vector3.Dot(texturePlanePosition, yAxis) + 0.5f
                );
            }
        }
        
        // FEZ meshes are counter-clockwise culled, which is the opposite of what most 3D software uses for front faces.
        public static IndexedPrimitives<VertexInstance, VertexType> WithReversedWindingIndices<VertexType>(
            this IndexedPrimitives<VertexInstance, VertexType> geometry
        )
        {
            var newGeometry = new IndexedPrimitives<VertexInstance, VertexType>()
            {
                PrimitiveType = geometry.PrimitiveType,
                Vertices = geometry.Vertices,
                Indices = new ushort[geometry.Indices.Length]
            };
            
            for (int i = 0; i < geometry.Indices.Length; i++)
            {
                var index = geometry.PrimitiveType switch
                {
                    PrimitiveType.TriangleList => i + (((i + 1) % 3) - 1),
                    PrimitiveType.TriangleStrip => geometry.Indices.Length - (i + 1),
                    _ => i
                };
                newGeometry.Indices[i] = geometry.Indices[index];
            }
            
            return newGeometry;
        }
    }
}
