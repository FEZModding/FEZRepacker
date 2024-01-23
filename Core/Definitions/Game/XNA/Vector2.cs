namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Vector2")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector2Reader")]
    public struct Vector2 : IEquatable<Vector2>
    {
        [XnbProperty]
        public float X { get; set; }

        [XnbProperty]
        public float Y { get; set; }

        public Vector2() { }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Vector2 other) =>
            this.X == other.X && this.Y == other.Y;
        public static bool operator ==(Vector2 left, Vector2 right) => left.Equals(right);
        public static bool operator !=(Vector2 left, Vector2 right) => !left.Equals(right);
    }
}
