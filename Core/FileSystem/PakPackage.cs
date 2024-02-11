using System.Text;

namespace FEZRepacker.Core.FileSystem
{
    public class PakPackage
    {
        private readonly List<PakFileRecord> entries;
        public List<PakFileRecord> Entries => entries;

        public PakPackage()
        {
            entries = new();
        }

        public PakPackage(Stream stream) : this()
        {
            LoadEntriesFrom(stream);
        }

        private void LoadEntriesFrom(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            entries.Clear();
            var entriesCount = reader.ReadUInt32();

            for (var i = 0; i < entriesCount; i++)
            {
                string path = reader.ReadString();
                int fileSize = (int)reader.ReadUInt32();
                var data = reader.ReadBytes(fileSize);

                var record = CreateEntry(path);
                using var recordStream = record.Open();
                recordStream.Write(data, 0, fileSize);
            }
        }

        public PakFileRecord CreateEntry(string localPath)
        {
            var record = new PakFileRecord(localPath);
            entries.Add(record);
            return record;
        }

        public void WriteTo(Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);

            writer.Write(entries.Count);
            foreach(var entry in entries)
            {
                writer.Write(entry.Path);
                writer.Write((uint)entry.Length);
                writer.Write(entry.Payload);
            }
        }

        public void SaveTo(string filePath)
        {
            using var stream = File.OpenWrite(filePath);
            WriteTo(stream);
        }

        public static PakPackage ReadFrom(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return new(stream);
        }
    }
}
