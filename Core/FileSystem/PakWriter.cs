﻿using System.Text;

namespace FEZRepacker.Core.FileSystem
{
    /// <summary>
    /// Allows creation of a FEZ PAK package.
    /// Use this for a memory-efficient creation process of PAK package.
    /// </summary>
    public class PakWriter : IDisposable
    {

        private readonly BinaryWriter writer;
        private readonly HashSet<string> writtenFilesRecords;

        private uint? lastWrittenFileCount = null;
        private uint currentFileCount = 0;

        public uint FileCount => currentFileCount;

        /// <param name="stream">A stream to write PAK package to.</param>
        public PakWriter(Stream stream)
        {
            writer = new BinaryWriter(stream, Encoding.UTF8, false);
            writtenFilesRecords = new HashSet<string>();

            WriteFileCount();
        }

        private void WriteFileCount()
        {
            if (lastWrittenFileCount == currentFileCount) return;

            var currentPosition = (int)writer.BaseStream.Position;
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(currentFileCount);
            lastWrittenFileCount = currentFileCount;

            if (currentPosition != 0)
            {
                writer.Seek(currentPosition, SeekOrigin.Begin);
            }
        }

        private bool AssureNoDuplicate(string fileRecord)
        {
            if (writtenFilesRecords.Contains(fileRecord))
            {
                return false;
            }
            writtenFilesRecords.Add(fileRecord);
            return true;
        }

        private void AppendNewFile(string name, Stream data)
        {
            writer.Write(name);
            writer.Write((uint)data.Length);
            data.CopyTo(writer.BaseStream);
            currentFileCount++;
        }

        /// <summary>
        /// Appends a file data at the end of PAK package.
        /// </summary>
        /// <param name="name">Path and name to identify the file data with in the package.</param>
        /// <param name="data">File data to store in the package.</param>
        /// <param name="filterExtension">Extension of the file, used to filter out duplicates.</param>
        /// <returns>True if the file was written successfully, false otherwise.</returns>
        public bool WriteFile(string name, Stream data, string filterExtension = "")
        {
            if (!AssureNoDuplicate(name + filterExtension))
            {
                return false;
            }
            AppendNewFile(name, data);
            return true;
        }

        public void Dispose()
        {
            WriteFileCount();

            writer.Dispose();
        }
    }
}