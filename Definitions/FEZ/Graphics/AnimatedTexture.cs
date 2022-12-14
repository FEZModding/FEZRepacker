using FEZRepacker.XNB.Attributes;
using Microsoft.Xna.Framework.Graphics;

namespace FezEngine.Content
{
    [XNBType("FezEngine.Readers.AnimatedTextureReader")]
    class AnimatedTexture
    {
        // Definition has been altered since the Texture property is
        // serialized in a different way than Texture reader does it

        [XNBProperty]
        public int AtlasWidth { get; set; }

        [XNBProperty]
        public int AtlasHeight { get; set; }

        [XNBProperty]
        public int FrameWidth { get; set; }

        [XNBProperty]
        public int FrameHeight { get; set; }

        [XNBProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] TextureData { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<FrameContent> Frames { get; set; }


        public AnimatedTexture()
        {
            Frames = new();
            TextureData = new byte[0];
        }
    }
}
