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
