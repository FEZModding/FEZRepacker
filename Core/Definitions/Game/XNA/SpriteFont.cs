
namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Graphics.SpriteFont, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.SpriteFontReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    public class SpriteFont
    {
        [XnbProperty(UseConverter = true)]
        public Texture2D Texture { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<Rectangle> GlyphBounds { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<Rectangle> Cropping { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<char> Characters { get; set; } = new();

        [XnbProperty]
        public int LineSpacing { get; set; }

        [XnbProperty]
        public float Spacing { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<Vector3> KerningData { get; set; } = new();

        [XnbProperty(UseConverter = true, Optional = true)]
        public char? DefaultCharacter { get; set; }
    }
}
