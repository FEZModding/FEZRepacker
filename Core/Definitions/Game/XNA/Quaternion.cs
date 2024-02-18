namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Quaternion")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.QuaternionReader")]
    public struct Quaternion : IEquatable<Quaternion>
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

        public Quaternion() {
            X = 0;
            Y = 0; 
            Z = 0;
            W = 1;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public bool Equals(Quaternion other) =>
            this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
        public static bool operator ==(Quaternion left, Quaternion right) => left.Equals(right);
        public static bool operator !=(Quaternion left, Quaternion right) => !left.Equals(right);

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode() + this.W.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion)) return false;
            return Equals((Quaternion)obj);
        }
    }
}
