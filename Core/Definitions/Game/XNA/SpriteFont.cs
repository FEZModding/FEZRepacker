
namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Graphics.SpriteFont")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.SpriteFontReader")]
    public class SpriteFont
    {
        [XnbProperty(UseConverter = true)]
        public Texture2D Texture { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<Rectangle> GlyphBounds { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<Rectangle> Cropping { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<char> Characters { get; set; }

        [XnbProperty]
        public int LineSpacing { get; set; }

        [XnbProperty]
        public float Spacing { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<Vector3> KerningData { get; set; }

        [XnbProperty(UseConverter = true, Optional = true)]
        public char DefaultCharacter { get; set; }

        public SpriteFont()
        {
            GlyphBounds = new List<Rectangle>();
            Cropping = new List<Rectangle>();
            Characters = new List<char>();
            KerningData = new List<Vector3>();
        }
    }
}
