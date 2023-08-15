using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FEZRepacker.Converter.FileSystem
{
    /// <summary>
    /// Allows accessing individual files in a FEZ PAK package contained in the given stream.
    /// </summary>
    public class PakReader : IDisposable
    {
        public struct FileInfo
        {
            public string Path;
            public uint Size;
            public string DetectedFileExtension;
            public Stream Data;
        }

        private BinaryReader reader;

        public uint FileCount { get; private set; }

        /// <summary>
        /// Initializes PAK package reader.
        /// </summary>
        /// <param name="stream">A stream to read a package from.</param>
        public PakReader(Stream stream)
        {
            reader = new BinaryReader(stream, Encoding.UTF8, false);

            FileCount = reader.ReadUInt32();
        }

        /// <summary>
        /// Enumerates files from the package.
        /// </summary>
        /// <remarks>Enumeration copies the content of the file into a buffer.</remarks>
        /// <returns>A list of files contained within the package.</returns>
        public IEnumerable<FileInfo> ReadFiles()
        {
            for (var i = 0; i < FileCount; i++)
            {
                string name = reader.ReadString();
                uint fileSize = reader.ReadUInt32();
                var data = reader.ReadBytes((int)fileSize);
                var stream = new MemoryStream(data);

                yield return new FileInfo
                {
                    Path = name,
                    Size = fileSize,
                    DetectedFileExtension = DetectFileExtension(data),
                    Data = stream
                };

                stream.Dispose();
            }
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        private static string DetectFileExtension(byte[] data)
        {
            var extension = "";

            if (data.Length >= 4)
            {
                if (data[0] == 'X' && data[1] == 'N' && data[2] == 'B')
                {
                    extension = ".xnb";
                }
                else if (data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S')
                {
                    extension = ".ogg";
                }
                else if (data[0] == 0x01 && data[1] == 0x09 && data[2] == 0xFF && data[3] == 0xFE)
                {
                    extension = ".fxc";
                }
            }

            return extension;
        }
    }
}
