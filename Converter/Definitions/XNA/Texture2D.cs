namespace FEZRepacker.Converter.Definitions.MicrosoftXna
{
    [XnbType("Microsoft.Xna.Framework.Graphics.Texture2D")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Texture2DReader")]
    internal class Texture2D
    {
        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public SurfaceFormat Format { get; set; }

        [XnbProperty]
        public int Width { get; set; }

        [XnbProperty]
        public int Height { get; set; }

        [XnbProperty]
        public int MipmapLevels { get; set; }

        // the format technically supports multiple mipmaps levels, which would alter how
        // this data is stored, but none of FEZ's original assets are using additional
        // mipmap levels, so I'm dropping support for it completely.
        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] TextureData { get; set; }

        public Texture2D()
        {
            TextureData = new byte[0];
            Format = SurfaceFormat.Color;
            MipmapLevels = 1;
        }
    }
}