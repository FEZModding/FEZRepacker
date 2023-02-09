using System.Text;

namespace FEZRepacker.Converter.PAK
{
    public class PakContainer : List<PakFile>
    {
        public PakContainer() : base()
        {

        }

        public static PakContainer Read(Stream stream)
        {
            var container = new PakContainer();

            using var streamReader = new BinaryReader(stream, Encoding.UTF8, false);

            uint filesCount = streamReader.ReadUInt32();

            for (var i = 0; i < filesCount; i++)
            {
                string name = streamReader.ReadString();
                uint fileSize = streamReader.ReadUInt32();
                byte[] data = streamReader.ReadBytes((int)fileSize);

                var file = new PakFile
                {
                    Path = name
                };

                using var fileWriter = file.Open();
                fileWriter.Write(data, (int)fileWriter.Position, data.Length);

                container.Add(file);
            }

            return container;
        }

        public void Save(Stream writeStream)
        {
            using var writer = new BinaryWriter(writeStream);
            writer.Write(Count);

            foreach (var file in this)
            {
                writer.Write(file.Path);

                using var fileStream = file.Open();
                writer.Write((int)fileStream.Length);
                fileStream.CopyTo(writeStream);
            }
        }
    }
}