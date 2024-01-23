namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Quaternion")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.QuaternionReader")]
    internal class Quaternion
    {
        [XnbProperty]
        public float X { get; set; }

        [XnbProperty]
        public float Y { get; set; }

        [XnbProperty]
        public float Z { get; set; }

        [XnbProperty]
        public float W { get; set; }

        public static Quaternion Identity => new Quaternion(0, 0, 0, 1);

        public Quaternion() { }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}
