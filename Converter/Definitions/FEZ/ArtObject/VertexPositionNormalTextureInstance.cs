using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry
{
    [XnbType("FezEngine.Structure.Geometry.VertexPositionNormalTextureInstance")]
    [XnbReaderType("FezEngine.Readers.VertexPositionNormalTextureInstanceReader")]
    internal class VertexPositionNormalTextureInstance
    {
        [XnbProperty]
        public Vector3 Position { get; set; }

        [XnbProperty]
        public byte NormalByte { get; set; }

        [XnbProperty]
        public Vector2 TextureCoordinate { get; set; }



        public Vector3 Normal
        {
            get
            {
                return ByteToNormal(NormalByte);
            }
            set
            {
                NormalByte = NormalToByte(value);
            }
        }

        public static Vector3 ByteToNormal(byte normalByte)
        {
            switch (normalByte % 6)
            {
                case 0: return new Vector3(-1f, 0f, 0f); // left
                case 1: return new Vector3(0f, -1f, 0f); // down
                case 2: return new Vector3(0f, 0f, -1f); // forward
                case 3: return new Vector3(1f, 0f, 0f); // right
                case 4: return new Vector3(0f, 1f, 0f); // up
                case 5: return new Vector3(0f, 0f, 1f); // backward
            }
            return Vector3.Zero;
        }

        public static byte NormalToByte(Vector3 normal)
        {
            byte normalByte = 0;
            float smallestDistance = 9f;
            for (byte i = 0; i < 6; i++)
            {
                float distance = (normal - ByteToNormal(i)).Length();
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    normalByte = 0;
                }
            }
            return normalByte;
        }


    }
}
