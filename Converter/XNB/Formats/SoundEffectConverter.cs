using FEZRepacker.Converter.Definitions.MicrosoftXna;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class SoundEffectConverter : XnbFormatConverter
    {
        public override XnbContentType[] TypesFactory => new XnbContentType[]
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
            var fileSize = 4 + (8 + data.FormatChunk.Length) + (8 + data.DataChunk.Length);
            outWriter.Write(fileSize);
            outWriter.Write("WAVE".ToCharArray());
            outWriter.Write("fmt ".ToCharArray());
            outWriter.Write(data.FormatChunk.Length);
            outWriter.Write(data.FormatChunk);
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
            while(inReader.BaseStream.Position < inReader.BaseStream.Length)
            {
                var chunkHeader = new string(inReader.ReadChars(4));
                var chunkLength = inReader.ReadInt32();
                if(chunkHeader == "fmt ")
                {
                    var byteArray = inReader.ReadBytes(chunkLength);
                    // Regular WAV fmt chunk is 16 bytes but SoundEffectReader reads additional
                    // 2 bytes for seemingly no reason (not assigned or used in any way by the reader).
                    // Filling it in just to make sure the game doesn't complain
                    if (chunkLength < 18)
                    {
                        Array.Resize(ref byteArray, 18);
                    }
                    soundEffect.FormatChunk = byteArray;
                }
                if(chunkHeader == "data")
                {
                    soundEffect.DataChunk = inReader.ReadBytes(chunkLength);
                }

                if(soundEffect.FormatChunk.Length > 0 && soundEffect.DataChunk.Length > 0)
                {
                    break;
                }
            }

            // not sure if loops are even used, but just to be safe, setting it to be the entire file
            int channelCount = (soundEffect.FormatChunk[3] << 8) + soundEffect.FormatChunk[2];
            soundEffect.LoopStart = 0;
            soundEffect.LoopLength = soundEffect.DataChunk.Length / Math.Max(1, channelCount);

            PrimaryContentType.Write(soundEffect, xnbWriter);
        }
    }
}
