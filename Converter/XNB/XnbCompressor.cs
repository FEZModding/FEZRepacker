using System.Text;

using Microsoft.Xna.Framework.Content;

namespace FEZRepacker.Converter.XNB
{
    public class XnbCompressor
    {
        public static Stream Decompress(Stream xnbStream)
        {
            var decompressedStream = new MemoryStream();

            if (!XnbHeader.TryRead(xnbStream, out var header) || (header.Flags & XnbFlags.Compressed) == 0)
            {
                xnbStream.Position = 0;
                xnbStream.CopyTo(decompressedStream);
            }
            else
            {
                header.Flags -= XnbFlags.Compressed;
                header.Write(decompressedStream);

                using var decompressedDataStream = new MemoryStream();

                using var xnbReader = new BinaryReader(xnbStream);
                LzxDecoder decoder = new LzxDecoder(16);

                int compressedSize = xnbReader.ReadInt32();
                int decompressedSize = xnbReader.ReadInt32();

                long startPos = xnbStream.Position;
                long pos = startPos;

                while (pos - startPos < compressedSize)
                {
                    // all of these shorts are big endian
                    int flag = xnbStream.ReadByte();
                    int frameSize, blockSize;
                    if (flag == 0xFF)
                    {
                        frameSize = (xnbStream.ReadByte() << 8) | xnbStream.ReadByte();
                        blockSize = (xnbStream.ReadByte() << 8) | xnbStream.ReadByte();
                        pos += 5;
                    }
                    else
                    {
                        frameSize = 0x8000;
                        blockSize = (flag << 8) | xnbStream.ReadByte();
                        pos += 2;
                    }


                    if (blockSize == 0 || frameSize == 0) break;

                    decoder.Decompress(xnbStream, blockSize, decompressedDataStream, frameSize);
                    pos += blockSize;

                    xnbStream.Position = pos;
                }

                if (decompressedDataStream.Position != decompressedSize)
                {
                    throw new Exception("XNBDecompressor failed!");
                }

                new BinaryWriter(decompressedStream).Write(decompressedSize);

                decompressedDataStream.Position = 0;
                decompressedDataStream.CopyTo(decompressedStream);
            }

            decompressedStream.Position = 0;
            return decompressedStream;
        }

        public static Stream Compress(Stream xnbStream)
        {
            throw new NotImplementedException();
        }
    }
}
