using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker
{
    using PAKDictionary = Dictionary<string, XNBFile>;

    class PAKFile
    {
        public bool Loaded { get; private set; }

        private PAKDictionary _files = new PAKDictionary();
        public PAKDictionary Files => _files;
        public int FileCount => _files.Count;

        public PAKFile(PAKDictionary files)
        {
            _files = files;
            Loaded = true;
        }

        public PAKFile(string path) => Load(path);

        public void Load(string path)
        {
            if (!File.Exists(path)) return;

            using var rawFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var rawFileReader = new BinaryReader(rawFileStream, Encoding.UTF8, false);
            
            uint filesCount = rawFileReader.ReadUInt32();

            for(var i = 0; i < filesCount; i++)
            {
                string name = rawFileReader.ReadString();
                uint fileSize = rawFileReader.ReadUInt32();
                byte[] data = rawFileReader.ReadBytes((int)fileSize);

                _files[name] = XNBFile.FromStream(new MemoryStream(data));
            }

            Loaded = Files.Count > 0;
        }

        public void Save(string path)
        {

        }
    }
}
