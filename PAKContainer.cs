using System.Text;
using System.Linq;

using FEZRepacker.XNB;

namespace FEZRepacker
{
    using PAKDictionary = List<PAKRecord>;
    struct PAKRecord
    {
        public string Name;
        public PAKFile File;
    };


    enum PAKUnpackMode
    {
        Convert,
        DecompressedXNB,
        RawXNB
    }

    enum PAKRemoveMode
    {
        RawOnly,
        XnbOnly,
        All
    }

    class PAKContainer
    {

        private PAKDictionary _files = new PAKDictionary();
        public PAKDictionary Files => _files;
        public int FileCount => _files.Count;


        public void Add(string name, PAKFile file)
        {
            _files.Add(new PAKRecord() { File = file, Name = name });
        }

        public int Count(string name)
        {
            return _files.Count(r => r.Name == name);
        }

        public void Remove(string name, PAKRemoveMode mode = PAKRemoveMode.All)
        {
            _files = _files.Where(r => r.Name != name 
                || ((r.File is XNBFile) && mode == PAKRemoveMode.RawOnly)
                || (!(r.File is XNBFile) && mode == PAKRemoveMode.XnbOnly)
            ).ToList();
        }

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

                PAKFile file;
                if(PAKFile.GetIdentifierFromData(data) == "XNB")
                {
                    file = XNBFile.FromData(data);
                }
                else
                {
                    file = PAKFile.FromData(data);
                }
                pak.Add(name, file);
            }
            return pak;
        }

        public void SavePak(string path)
        {
            using var rawFileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var rawFileWriter = new BinaryWriter(rawFileStream, Encoding.UTF8, false);

            rawFileWriter.Write(_files.Count);

            foreach(PAKRecord record in _files)
            {
                rawFileWriter.Write(record.Name);
                rawFileWriter.Write(record.File.GetSize());
                record.File.Write(rawFileWriter);
            }

            rawFileWriter.Close();
            rawFileStream.Close();
        }

        public void LoadContent(string path)
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach(string filepath in files)
            {
                string actualName = Path.ChangeExtension(Path.GetRelativePath(path, filepath), null);

                if (Count(actualName) > 0)
                {
                    Console.WriteLine($"PAK file already contains \"{actualName}\" and it will be overwritten if the type matches!");
                }

                PAKFile file;

                // try to convert it into xnb file
                if(XNBContentConvert.TryLoad(filepath, out XNBFile xnbFile))
                {
                    file = xnbFile;
                }
                else
                {
                    using var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fileStream.Length];
                    fileStream.Read(data, 0, data.Length);
                    file = PAKFile.FromData(data);
                }

                Remove(actualName, (file is XNBFile) ? PAKRemoveMode.XnbOnly : PAKRemoveMode.RawOnly);

                Add(actualName, file);
            }
        }

        public void SaveContent(string path, PAKUnpackMode mode = PAKUnpackMode.Convert)
        {
            foreach (PAKRecord record in Files)
            {
                var file = record.File;
                // preparing a directory for given file
                string fullPath = path + "\\" + record.Name;
                var dirName = Path.GetDirectoryName(fullPath);
                if (dirName != null && !Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

                bool savedXNBFile = false;

                // try custom xnb content saving
                if(file is XNBFile)
                {
                    XNBFile xnbfile = ((XNBFile)file);

                    if(mode == PAKUnpackMode.Convert) savedXNBFile = XNBContentConvert.TrySave(xnbfile, fullPath);

                    if (!savedXNBFile && mode != PAKUnpackMode.RawXNB) file = xnbfile.Decompressed();
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
