using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class SoundEffectConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<SoundEffect>(this),
            new ByteArrayContentType(this)
        };
        public override string FileFormat => ".wav";

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            XnbContentType primaryType = PrimaryContentType;

            SoundEffect data = (SoundEffect)primaryType.Read(xnbReader);

            outWriter.Write("RIFF".ToCharArray());
            var fileSize = 4 + (8 + 18) + (8 + data.DataChunk.Length);
            outWriter.Write(fileSize);
            outWriter.Write("WAVE".ToCharArray());
            outWriter.Write("fmt ".ToCharArray());
            outWriter.Write(16);
            outWriter.Write(data.FormatType);
            outWriter.Write(data.ChannelCount);
            outWriter.Write(data.SampleFrequency);
            outWriter.Write(data.BytesPerSecond);
            outWriter.Write(data.BlockAlignment);
            outWriter.Write(data.BitsPerSample);
            outWriter.Write("data".ToCharArray());
            outWriter.Write(data.DataChunk.Length);
            outWriter.Write(data.DataChunk);
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            var soundEffect = new SoundEffect();

            var riffHeader = new string(inReader.ReadChars(4));
            var fileSize = inReader.ReadInt32();
            var waveHeader = new string(inReader.ReadChars(4));
            if (riffHeader != "RIFF" || waveHeader != "WAVE")
            {
                throw new InvalidDataException("WAV file has incorrect header");
            }
            while (inReader.BaseStream.Position < inReader.BaseStream.Length)
            {
                var chunkHeader = new string(inReader.ReadChars(4));
                var chunkLength = inReader.ReadInt32();

                if (chunkHeader == "fmt ")
                {
                    soundEffect.FormatChunkSize = 18;
                    soundEffect.FormatType = inReader.ReadInt16();
                    soundEffect.ChannelCount = inReader.ReadInt16();
                    soundEffect.SampleFrequency = inReader.ReadInt32();
                    soundEffect.BytesPerSecond = inReader.ReadInt32();
                    soundEffect.BlockAlignment = inReader.ReadInt16();
                    soundEffect.BitsPerSample = inReader.ReadInt16();
                    soundEffect.ExtraParameter = 0;
                    inReader.ReadBytes(chunkLength - 16);
                }
                else
                {
                    var chunkData = inReader.ReadBytes(chunkLength);
                    if (chunkHeader == "data") soundEffect.DataChunk = chunkData;
                }

                if (soundEffect.FormatChunkSize > 0 && soundEffect.DataChunk.Length > 0)
                {
                    break;
                }
            }

            // Additional verification - bits-per-sample values that are not divisible by 16
            // aren't handled by XNA properly. Uneven values (like 24-bit PCMs) causes corrupted sound.
            if (soundEffect.FormatType != 2 && soundEffect.BitsPerSample % 16 != 0)
            {
                throw new InvalidDataException($"PCM WAV file bit resolution must be divisible by 16 ({soundEffect.BitsPerSample}-bit samples detected)");
            }

            // not sure if loops are even used, but just to be safe, setting it to be the entire file
            soundEffect.LoopStart = 0;
            soundEffect.LoopLength = soundEffect.DataChunk.Length / Math.Max(1, (int)soundEffect.ChannelCount);

            PrimaryContentType.Write(soundEffect, xnbWriter);
        }
    }
}
