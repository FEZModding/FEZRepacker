using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void SavePak()
        {
            // TODO: logic here
        }

        public static PAKContainer LoadContent(string path)
        {
            PAKContainer pak = new PAKContainer();

            // TODO: logic here

            return pak;
        }
        public void SaveContent(string path)
        {
            foreach ((var name, var file) in Files)
            {

                bool savedXNB = false;

                if (file is XNBFile)
                {
                    var xnbFile = (XNBFile)file;

                    xnbFile.Decompress();

                    if (xnbFile.IsValid)
                    {
                        XNBContent content = xnbFile.ReadXNBContent();

                        savedXNB = true;
                    }
                    
                }
                
                if(!savedXNB)
                {

                }
                // todo - further logic here
            }



            //if (!Directory.Exists(unpackPath)) Directory.CreateDirectory(unpackPath);
            //File.WriteAllBytes($"{unpackPath}/test.bin", testFile.Content);
        }
    }
}
