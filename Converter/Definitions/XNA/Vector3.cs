using System.Numerics;

namespace FEZRepacker.Converter.Definitions.MicrosoftXna
{
    [XnbType("Microsoft.Xna.Framework.Vector3")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector3Reader")]
    internal class XNAVector3
    {
        [XnbProperty]
        public float X { get; set; }

        [XnbProperty]
        public float Y { get; set; }

        [XnbProperty]
        public float Z { get; set; }

        public XNAVector3() { }

        public XNAVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Vector3 (XNAVector3 v) => new Vector3 (v.X, v.Y, v.Z);
        public static implicit operator XNAVector3(Vector3 v) => new XNAVector3(v.X, v.Y, v.Z);
    }
}
