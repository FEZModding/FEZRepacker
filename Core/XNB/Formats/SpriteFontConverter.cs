using System.Text;

using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.Definitions.FezEngine.Structure.Geometry;
using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Formats.Json.CustomStructures;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

using Rectangle = FEZRepacker.Converter.Definitions.MicrosoftXna.Rectangle;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class SpriteFontConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<SpriteFont>(this),
            new GenericContentType<Texture2D>(this),
            new EnumContentType<SurfaceFormat>(this),
            new ByteArrayContentType(this),
            new ListContentType<Rectangle>(this, true),
            new GenericContentType<Rectangle>(this),
            new ListContentType<char>(this),
            new CharContentType(this),
            new ListContentType<XNAVector3>(this, true),
            new GenericContentType<XNAVector3>(this),
        };
        public override string FileFormat => ".fezfont";

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            SpriteFont font = (SpriteFont)PrimaryContentType.Read(xnbReader);

            var texture = SaveTexture(font);
            var data = SaveAdditionalData(font);

            var bundle = new FileBundle(FileFormat);
            bundle.AddFile(texture, ".png");
            bundle.AddFile(data, ".json");
            return bundle;
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            SpriteFont font = new SpriteFont();

            foreach(var file in bundle.Files)
            {
                if (file.Extension == ".png") LoadTexture(file.Data, ref font);
                if (file.Extension == ".json") LoadAdditionalData(file.Data, ref font);
            }

            PrimaryContentType.Write(font, xnbWriter);
        }

        private static Stream SaveTexture(SpriteFont font)
        {
            var memoryStream = new MemoryStream();

            if (font.Texture.TextureData.Length == 0)
            {
                return memoryStream;
            }
            
            using var image = TextureConverter.ImageFromTexture2D(font.Texture);

            image.Save(memoryStream, new PngEncoder());
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private static Stream SaveAdditionalData(SpriteFont font)
        {
            var json = CustomJsonSerializer.Serialize(SpriteFontJsonData.FromSpriteFont(font));
            return new MemoryStream(Encoding.UTF8.GetBytes(json));
        }

        private static void LoadTexture(Stream textureStream, ref SpriteFont font)
        {
            if (textureStream == null) return;

            using var image = Image.Load<Rgba32>(textureStream);

            font.Texture = TextureConverter.ImageToTexture2D(image);
        }


        private static void LoadAdditionalData(Stream jsonStream, ref SpriteFont font)
        {
            using var jsonReader = new BinaryReader(jsonStream, Encoding.UTF8, true);
            string json = new string(jsonReader.ReadChars((int)jsonStream.Length));
            var data = CustomJsonSerializer.Deserialize<SpriteFontJsonData>(json);

            data.ToSpriteFont(ref font);
        }
    }
}
