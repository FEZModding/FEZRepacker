namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Color, Microsoft.Xna.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
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
