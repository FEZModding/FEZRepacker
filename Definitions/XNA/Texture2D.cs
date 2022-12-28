using FEZRepacker.XNB.Attributes;

namespace Microsoft.Xna.Framework.Graphics
{
    [XNBType("Microsoft.Xna.Framework.Content.Texture2DReader")]
    class Texture2D
    {
        [XNBProperty(UseConverter = true, SkipIdentifier = true)]
        public SurfaceFormat Format { get; set; }

        [XNBProperty]
        public int Width { get; set; }

        [XNBProperty]
        public int Height { get; set; }

        [XNBProperty]
        public int MipmapLevels { get; set; }

        // the format technically supports multiple mipmaps levels, which would alter how
        // this data is stored, but none of FEZ's original assets are using additional
        // mipmap levels, so I'm dropping support for it completely.
        [XNBProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] TextureData { get; set; }

        public Texture2D()
        {
            TextureData= new byte[0];
            Format = SurfaceFormat.Color;
            MipmapLevels = 1;
        }
    }
}
