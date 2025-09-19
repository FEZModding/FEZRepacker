namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Vector4")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector4Reader")]
    public struct Vector4 : IEquatable<Vector4>
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

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public bool Equals(Vector4 other) =>
            this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
        public static bool operator ==(Vector4 left, Vector4 right) => left.Equals(right);
        public static bool operator !=(Vector4 left, Vector4 right) => !left.Equals(right);
        
        public System.Numerics.Vector4 ToNumeric() => new(this.X, this.Y, this.Z, this.W);
        public static Vector4 FromNumeric(System.Numerics.Vector4 numeric) => new(numeric.X, numeric.Y, numeric.Z, numeric.W);

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2 ^ this.Z.GetHashCode() >> 2 ^ this.W.GetHashCode() << 4;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector4)) return false;
            return Equals((Vector4)obj);
        }
    }
}
