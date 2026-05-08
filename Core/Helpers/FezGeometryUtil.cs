using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Helpers
{
    internal static class FezGeometryUtil
    {
        /// Recalculates texture coordinates to match what game does with them during loading process.
        public static void RecalculateCubemapTexCoords<VertexType>(
            IndexedPrimitives<VertexInstance, VertexType> geometry, Vector3 geometryBounds, bool widen
        ) {
            // keeping arithmetics vaguely similar to the one in game to prevent number mismatch after conversion
            foreach (var vertex in geometry.Vertices)
            {
                (float textureOffset, Vector3 xAxis, Vector3 yAxis) = vertex.NormalByte switch
                {
                    5 => (0, new Vector3(1, 0, 0), new Vector3(0, 1, 0)), // front
                    3 => (0.25f, new Vector3(0, 0, -1), new Vector3(0, 1, 0)), // right
                    2 => (0.375f, new Vector3(-1, 0, 0), new Vector3(0, 1, 0)), // back
                    0 => (0.375f, new Vector3(0, 0, 1), new Vector3(0, 1, 0)), // left
                    4 => (0.5f, new Vector3(1, 0, 0), new Vector3(0, 0, 1)), // top
                    1 => (0.625f, new Vector3(1, 0, 0), new Vector3(0, 0, 1)), // bottom
                    _ => (0, new Vector3(), new Vector3())
                };

                var texturePlanePosition = (Vector3.One - vertex.Normal) * (vertex.Position / geometryBounds) * 2f + vertex.Normal;
                var texturePlanePositionNormalized = texturePlanePosition / 2f + new Vector3(0.5f, 0.5f, 0.5f);
                float uCoordinate = Vector3.Dot(xAxis, texturePlanePositionNormalized);
                float vCoordinate = Vector3.Dot(yAxis, texturePlanePositionNormalized);
                if (vertex.NormalByte != 4)
                {
                    vCoordinate = 1.0f - vCoordinate;
                }

                float finalUCoordinate = (textureOffset + uCoordinate / 8f) * (widen ? 1.3333334f : 1.0f);
                vertex.TextureCoordinate = new Vector2(finalUCoordinate, vCoordinate);
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
