namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Color")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.ColorReader")]
    public class Color
    {
        [XnbProperty]
        public byte R { get; set; }

        [XnbProperty]
        public byte G { get; set; }

        [XnbProperty]
        public byte B { get; set; }

        [XnbProperty]
        public byte A { get; set; }

        public Color() { }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
