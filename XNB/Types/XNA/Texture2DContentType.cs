using System.Drawing;

using Microsoft.Xna.Framework.Graphics;

namespace FEZRepacker.XNB.Types.XNA
{
    class Texture2DContentType : XNBContentType<Texture2D>
    {
        public Texture2DContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.Texture2DReader";

        public override object Read(BinaryReader reader)
        {
            Texture2D txt = new Texture2D();
            txt.Format = (SurfaceFormat)reader.ReadInt32();
            txt.Width = reader.ReadInt32();
            txt.Height = reader.ReadInt32();
            txt.MipmapLevels = reader.ReadInt32();
            // barely any texture (no significant one) is using mipmapping
            // so I'm just ignoring it completely
            txt.MipmapLevels = 1;

            int dataSize = reader.ReadInt32();
            txt.TextureData = reader.ReadBytes(dataSize);

            return txt;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Texture2D txt = (Texture2D)data;

            writer.Write((int)txt.Format);
            writer.Write(txt.Width);
            writer.Write(txt.Height);
            writer.Write(txt.MipmapLevels);
            writer.Write(txt.TextureData.Length);
            writer.Write(txt.TextureData);
        }
    }
}
