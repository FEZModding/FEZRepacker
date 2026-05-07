namespace FEZRepacker.Core.Definitions.Game.Graphics
{
    [XnbType("FezEngine.Structure.AnimatedTexture, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.AnimatedTextureReader, FezEngine")]
    public class AnimatedTexture
    {
        // Definition has been altered since the Texture property is
        // serialized in a different way than Texture reader does it

        [XnbProperty]
        public int AtlasWidth { get; set; }

        [XnbProperty]
        public int AtlasHeight { get; set; }

        [XnbProperty]
        public int FrameWidth { get; set; }

        [XnbProperty]
        public int FrameHeight { get; set; }

        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] TextureData { get; set; } = { };

        [XnbProperty(UseConverter = true)]
        public List<FrameContent> Frames { get; set; } = new();
    }
}
