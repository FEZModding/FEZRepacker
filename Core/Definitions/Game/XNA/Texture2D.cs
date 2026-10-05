namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Graphics.Texture2D, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Texture2DReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553", UseCustomSerializer = true)]
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

        // Keep the first level separate for converters that operate on the full-size image.
        [XnbProperty(UseConverter = true, SkipIdentifier = true)]
        public byte[] TextureData { get; set; } = { };

        // Preserve the remaining levels in XNB order so serialization can write the full chain.
        public byte[][] AdditionalMipmaps { get; set; } = [];
    }
}
