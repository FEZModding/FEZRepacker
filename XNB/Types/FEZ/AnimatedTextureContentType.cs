using System.Drawing;
using FezEngine.Structure;
using Microsoft.Xna.Framework.Graphics;

namespace FEZRepacker.XNB.Types.XNA
{
    class AnimatedTextureContentType : XNBContentType<AnimatedTexture>
    {
        public AnimatedTextureContentType(XNBContentConverter converter) : base(converter) { }
        public override TypeAssemblyQualifier Name => "FezEngine.Readers.AnimatedTextureReader";

        public override object Read(BinaryReader reader)
        {
            AnimatedTexture txt = new AnimatedTexture();

            txt.Texture.Format = SurfaceFormat.Color;
            txt.Texture.MipmapLevels = 1;
            txt.Texture.Width = reader.ReadInt32();
            txt.Texture.Height = reader.ReadInt32();

            txt.FrameWidth = reader.ReadInt32();
            txt.FrameHeight = reader.ReadInt32();

            txt.Texture.TextureData = reader.ReadBytes(reader.ReadInt32());

            txt.Frames = Converter.ReadType<List<FrameContent>>(reader) ?? txt.Frames;

            return txt;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            AnimatedTexture txt = (AnimatedTexture)data;


        }
    }
}
