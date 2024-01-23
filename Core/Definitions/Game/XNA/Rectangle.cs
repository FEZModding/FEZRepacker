namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Rectangle")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.RectangleReader")]
    internal class Rectangle
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
