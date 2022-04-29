using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;

namespace FEZRepacker
{
    class XNBDecompressor
    {
        // no idea what this number means, but it works...
        public static int COMPRESSED_PROLOGUE_SIZE = 14;

        public static byte[] Decompress(byte[] inData)
        {
            LzxDecoder decoder = new LzxDecoder(16);

            int compressedSize = inData.Length - COMPRESSED_PROLOGUE_SIZE;
            int decompressedSize = BitConverter.ToInt32(inData, 0);

            var dataStream = new MemoryStream(inData);
            dataStream.Position += 4; // skip the size
            var decompressedStream = new MemoryStream(decompressedSize);

            long startPos = dataStream.Position;
            long pos = startPos;

            while (pos - startPos < compressedSize)
            {
                // all of these shorts are big endian
                int flag = dataStream.ReadByte();
                int frameSize, blockSize;
                if (flag == 0xFF)
                {
                    frameSize = (dataStream.ReadByte() << 8) | dataStream.ReadByte();
                    blockSize = (dataStream.ReadByte() << 8) | dataStream.ReadByte();
                    pos += 5;
                }
                else
                {
                    frameSize = 0x8000;
                    blockSize = (flag << 8) | dataStream.ReadByte();
                    pos += 2;
                }
                    

                if (blockSize == 0 || frameSize == 0) break;

                decoder.Decompress(dataStream, blockSize, decompressedStream, frameSize);
                pos += blockSize;

                dataStream.Position = pos;
            }

            if (decompressedStream.Position != decompressedSize)
            {
                throw new Exception("XNBDecompressor failed!");
            }

            decompressedStream.Position = 0;
            byte[] outData = new byte[decompressedSize];
            decompressedStream.Read(outData, 0, decompressedSize);

            return outData;
        }
    }
}
