namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Vector2")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector2Reader")]
    public class Vector2
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
    }
}
