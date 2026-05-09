using System.Text;

using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers;

namespace FEZRepacker.Core.Conversion.Formats
{
    internal class SoundEffectConverter : FormatConverter<SoundEffect>
    {
        public override string FileFormat => ".wav";

        public override FileBundle ConvertTyped(SoundEffect data)
        {
            var outStream = new MemoryStream();
            using var outWriter = new BinaryWriter(outStream, Encoding.UTF8, true);

            outWriter.Write("RIFF".ToCharArray());
            const int headerSize = 104; // RIFF + WAVE + fmt chunk + smpl chunk + data chunk header
            var fileSize = headerSize + data.DataChunk.Length;
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
            outWriter.Write("smpl".ToCharArray());
            outWriter.WriteInts(52, 0, 0, 222675, 60, 0, 0, 0, 1, 0);
            outWriter.WriteInts(0, 0);
            outWriter.Write(data.LoopStart);
            outWriter.Write(data.LoopStart + data.LoopLength - 1);
            outWriter.WriteInts(0, 0);
            outWriter.Write("data".ToCharArray());
            outWriter.Write(data.DataChunk.Length);
            outWriter.Write(data.DataChunk);

            outStream.Seek(0, SeekOrigin.Begin);
            return FileBundle.Single(outStream, FileFormat);
        }

        public override SoundEffect DeconvertTyped(FileBundle bundle)
        {
            var soundEffect = new SoundEffect();

            using var inReader = new BinaryReader(bundle.RequireData(""), Encoding.UTF8, true);

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
                else if (chunkHeader == "smpl")
                {
                    inReader.ReadBytes(28);
                    var sampleLoops = inReader.ReadInt32();
                    var extraBytesCount = inReader.ReadInt32();
                    for (int i = 0; i < sampleLoops; i++)
                    {
                        inReader.ReadBytes(8);
                        var loopStart = inReader.ReadInt32();
                        var loopEnd = inReader.ReadInt32();
                        inReader.ReadBytes(8);

                        if (i == 0)
                        {
                            soundEffect.LoopStart = loopStart;
                            soundEffect.LoopLength = loopEnd - loopStart + 1;
                        }
                    }
                    inReader.ReadBytes(extraBytesCount);
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
            
            soundEffect.DurationMs = (int)(((long)soundEffect.DataChunk.Length * 1000) / soundEffect.BytesPerSecond);

            return soundEffect;
        }
    }
}
