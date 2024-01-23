namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Vector4")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector4Reader")]
    public class Vector4
    {
        [XnbProperty]
        public float X { get; set; }

        [XnbProperty]
        public float Y { get; set; }

        [XnbProperty]
        public float Z { get; set; }

        [XnbProperty]
        public float W { get; set; }

        public Vector4() { }

        public Vector4(float w, float x, float y, float z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }
    }
}
