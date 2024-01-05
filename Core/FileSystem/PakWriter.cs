using System;
using System.Collections.Generic;
using System.Text;

namespace FEZRepacker.Converter.FileSystem
{
    /// <summary>
    /// Allows creation of a FEZ PAK package.
    /// </summary>
    public class PakWriter : IDisposable
    {
        private BinaryWriter writer;

        private uint lastWrittenFileCount = 0;
        private uint currentFileCount = 0;

        public uint FileCount => currentFileCount;

        /// <summary>
        /// Initializes a PAK writer.
        /// </summary>
        /// <param name="stream">A stream to write PAK package to.</param>
        public PakWriter(Stream stream)
        {
            writer = new BinaryWriter(stream, Encoding.UTF8, false);

            // write 0 for now. it'll be replaced once we finalize it
            writer.Write(lastWrittenFileCount);
        }

        /// <summary>
        /// Appends a file data at the end of PAK package.
        /// </summary>
        /// <param name="name">Path and name to identify the file data with in the package.</param>
        /// <param name="data">File data to store in the package.</param>
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
