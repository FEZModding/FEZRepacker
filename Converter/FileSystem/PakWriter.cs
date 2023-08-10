using System;
using System.Collections.Generic;
using System.Text;

namespace FEZRepacker.Converter.FileSystem
{
    public class PakWriter : IDisposable
    {
        private BinaryWriter writer;

        private uint lastWrittenFileCount = 0;
        private uint currentFileCount = 0;

        public uint FileCount => currentFileCount;

        public PakWriter(Stream stream)
        {
            writer = new BinaryWriter(stream, Encoding.UTF8, false);

            // write 0 for now. it'll be replaced once we finalize it
            writer.Write(lastWrittenFileCount);
        }

        public void WriteFile(string name, Stream data)
        {
            writer.Write(name);
            writer.Write((uint)data.Length);
            data.CopyTo(writer.BaseStream);
            currentFileCount++;
        }

        public void WriteFileCount()
        {
            if (lastWrittenFileCount == currentFileCount) return;

            var currentPosition = (int)writer.BaseStream.Position;
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(currentFileCount);
            lastWrittenFileCount = currentFileCount;

            writer.Seek(currentPosition, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            WriteFileCount();

            writer.Dispose();
        }
    }
}
