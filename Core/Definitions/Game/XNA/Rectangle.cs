namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Rectangle, Microsoft.Xna.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.RectangleReader")]
    public class Rectangle
    {
        [XnbProperty]
        public int X { get; set; }

        [XnbProperty]
        public int Y { get; set; }

        [XnbProperty]
        public int Width { get; set; }

        [XnbProperty]
        public int Height { get; set; }

        public Rectangle()
        {
            X = 0;
            Y = 0;
            Width = 0; 
            Height = 0;
        }
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
