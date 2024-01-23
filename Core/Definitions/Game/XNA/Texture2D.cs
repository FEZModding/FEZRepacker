namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Graphics.Texture2D")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Texture2DReader")]
    public class Texture2D
    {
        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public SurfaceFormat Format { get; set; } = SurfaceFormat.Color;

        [XnbProperty]
        public int Width { get; set; }

        [XnbProperty]
        public int Height { get; set; }

        [XnbProperty]
        public int MipmapLevels { get; set; } = 1;

        // the format technically supports multiple mipmaps levels, which would alter how
        // this data is stored, but none of FEZ's original assets are using additional
        // mipmap levels, so I'm dropping support for it completely.
        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] TextureData { get; set; } = { };
    }
}
