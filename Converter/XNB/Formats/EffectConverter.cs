using FEZRepacker.Converter.Definitions.MicrosoftXna;
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

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            XnbContentType primaryType = PrimaryContentType;

            Effect data = (Effect)primaryType.Read(xnbReader);
            outWriter.Write(data.Data.Length);
            outWriter.Write(data.Data);
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            var effect = new Effect();
            effect.Data = inReader.ReadBytes(inReader.ReadInt32());
            PrimaryContentType.Write(effect, xnbWriter);
        }
    }
}
