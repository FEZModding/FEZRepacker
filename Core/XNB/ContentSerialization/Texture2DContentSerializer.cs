using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.XNB.ContentSerialization
{
    internal sealed class Texture2DContentSerializer : XnbContentSerializer<Texture2D>
    {
        private static readonly XnbAssemblyQualifier ReaderName = 
            XnbAssemblyQualifier.TryGetFromXnbReaderType(typeof(Texture2D)) 
            ?? throw new InvalidOperationException();

        public override XnbAssemblyQualifier Name => ReaderName;

        public override object Deserialize(XnbContentReader reader)
        {
            var texture = new Texture2D
            {
                Format = (SurfaceFormat)reader.ReadInt32(),
                Width = reader.ReadInt32(),
                Height = reader.ReadInt32(),
                MipmapLevels = reader.ReadInt32()
            };

            if (texture.MipmapLevels < 1)
            {
                throw new XnbSerializationException("A texture must contain at least one mipmap level.");
            }

            var length = reader.ReadInt32();
            var data = reader.ReadBytes(length);
            texture.TextureData = data;

            texture.AdditionalMipmaps = new byte[texture.MipmapLevels - 1][];
            for (var level = 0; level < texture.AdditionalMipmaps.Length; level++)
            {
                var length1 = reader.ReadInt32();
                var data1 = reader.ReadBytes(length1);
                texture.AdditionalMipmaps[level] = data1;
            }

            return texture;
        }

        public override void Serialize(object data, XnbContentWriter writer)
        {
            var texture = (Texture2D)data;
            if (texture.TextureData == null)
            {
                throw new InvalidOperationException("Cannot serialize required XNB property Texture2D.TextureData because it is null.");
            }
            if (texture.AdditionalMipmaps == null)
            {
                throw new InvalidOperationException("Texture2D.AdditionalMipmapData cannot be null.");
            }
            if (texture.MipmapLevels < 1 || texture.MipmapLevels - 1 != texture.AdditionalMipmaps.Length)
            {
                throw new XnbSerializationException("Texture mipmap count does not match its pixel data.");
            }
            if (texture.AdditionalMipmaps.Any(mipmap => mipmap == null))
            {
                throw new XnbSerializationException("Texture mipmap pixel data cannot be null.");
            }

            writer.Write((int)texture.Format);
            writer.Write(texture.Width);
            writer.Write(texture.Height);
            writer.Write(texture.MipmapLevels);
            writer.Write(texture.TextureData.Length);
            writer.Write(texture.TextureData);

            foreach (var mipmap in texture.AdditionalMipmaps)
            {
                writer.Write(mipmap.Length);
                writer.Write(mipmap);
            }
        }
    }
}
