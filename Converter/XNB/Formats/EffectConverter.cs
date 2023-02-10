using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class EffectConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<Effect>(this),
            new ByteArrayContentType(this)
        };
        public override string FileFormat => ".fxb";

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            XnbContentType primaryType = PrimaryContentType;

            Effect data = (Effect)primaryType.Read(xnbReader);

            var outStream = new MemoryStream();
            var outWriter = new BinaryWriter(outStream);

            outWriter.Write(data.Data.Length);
            outWriter.Write(data.Data);

            outStream.Seek(0, SeekOrigin.Begin);
            return FileBundle.Single(outStream, FileFormat);
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            var effect = new Effect();
            var inReader = new BinaryReader(bundle.GetData());
            effect.Data = inReader.ReadBytes(inReader.ReadInt32());
            PrimaryContentType.Write(effect, xnbWriter);
        }
    }
}
