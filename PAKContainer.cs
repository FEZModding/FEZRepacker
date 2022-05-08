using System.Text;

using FEZRepacker.XNB;

namespace FEZRepacker
{
    using PAKDictionary = Dictionary<string, PAKFile>;

    class PAKContainer
    {

        private PAKDictionary _files = new PAKDictionary();
        public PAKDictionary Files => _files;
        public int FileCount => _files.Count;

        public static PAKContainer LoadPak(string path)
        {
            if (!File.Exists(path)) return null;

            PAKContainer pak = new PAKContainer();

            using var rawFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var rawFileReader = new BinaryReader(rawFileStream, Encoding.UTF8, false);
            
            uint filesCount = rawFileReader.ReadUInt32();

            for(var i = 0; i < filesCount; i++)
            {
                string name = rawFileReader.ReadString();
                uint fileSize = rawFileReader.ReadUInt32();
                byte[] data = rawFileReader.ReadBytes((int)fileSize);

                pak._files[name] = PAKFile.FromData(data);
                if(pak._files[name].Identifier == "XNB")
                {
                    pak._files[name] = XNBFile.FromData(data);
                }
            }
            return pak;
        }

        public void SavePak(string path)
        {
            using var rawFileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var rawFileWriter = new BinaryWriter(rawFileStream, Encoding.UTF8, false);

            rawFileWriter.Write(_files.Count);

            foreach((string name, PAKFile file) in _files)
            {
                rawFileWriter.Write(file.GetSize());
                file.Write(rawFileWriter);
            }

            rawFileWriter.Close();
            rawFileStream.Close();
        }

        public static PAKContainer LoadContent(string path)
        {
            PAKContainer pak = new PAKContainer();

            // TODO: logic here

            return pak;
        }
        public void SaveContent(string path)
        {
            foreach (var name in Files.Keys)
            {
                var file = Files[name];
                // preparing a directory for given file
                string fullPath = path + "\\" + name;
                var dirName = Path.GetDirectoryName(fullPath);
                if (dirName != null && !Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

                bool savedXNBFile = false;

                

                // try custom xnb content saving
                if(file is XNBFile)
                {
                    XNBFile xnbfile = ((XNBFile)file);

                    savedXNBFile = XNBContentConvert.TrySave(xnbfile, fullPath);

                    if (!savedXNBFile) file = xnbfile.Decompressed();
                }

                // if that fails, just do it normally
                if (!savedXNBFile)
                {
                    string extension = file.GetExtension();

                    using var fileStream = new FileStream($"{fullPath}.{extension}", FileMode.Create, FileAccess.Write);
                    using var fileWriter = new BinaryWriter(fileStream, Encoding.UTF8, false);

                    file.Write(fileWriter);

                    fileWriter.Close();
                    fileStream.Close();
                }
                
            }
        }
    }
}
