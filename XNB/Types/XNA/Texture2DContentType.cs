using System.Drawing;

using Microsoft.Xna.Framework.Graphics;

namespace FEZRepacker.XNB.Types.XNA
{
    class Texture2DContentType : XNBContentType<Texture2D>
    {
        public Texture2DContentType(XNBContentConverter converter) : base(converter) { }
        public override TypeAssemblyQualifier Name => "Microsoft.Xna.Framework.Content.Texture2DReader";

        private static readonly SurfaceFormat EXPECTED_FORMAT = SurfaceFormat.Color;
        private static readonly SurfaceFormat EXPECTED_FORMAT_ALT = SurfaceFormat.Dxt1;
        private static readonly int EXPECTED_MIPMAP_LEVELS = 1;

        public override object Read(BinaryReader reader)
        {
            Texture2D txt = new Texture2D();
            txt.Format = (SurfaceFormat)reader.ReadInt32();
            txt.Width = reader.ReadInt32();
            txt.Height = reader.ReadInt32();
            txt.MipmapLevels = reader.ReadInt32();

            // FEZ textures should have specific surface format and only one mipmap level
            if(txt.Format != EXPECTED_FORMAT && txt.Format != EXPECTED_FORMAT_ALT)
            {
                throw new InvalidDataException("Texture2D has unexpected surface format.");
            }
            if (txt.MipmapLevels != EXPECTED_MIPMAP_LEVELS)
            {
                throw new InvalidDataException("Texture2D has unexpected mipmap levels count.");
            }

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
            writer.Write(txt.TextureData);
        }
    }
}
