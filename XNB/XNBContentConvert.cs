using FEZRepacker.XNB.Converters.Files;
using System.Linq;
using System.Text;

namespace FEZRepacker.XNB
{
    static class XNBContentConvert
    {
        private static Dictionary<TypeAssemblyQualifier, XNBContentConverter> _convertersList = new();
        static XNBContentConvert()
        {
            AddConverter(new TextStorageConverter());
            AddConverter(new LevelConverter());
            AddConverter(new TextureConverter());
            AddConverter(new AnimatedTextureConverter());
        }

        private static void AddConverter(XNBContentConverter converter)
        {
            if (converter.Types.Length == 0) return;
            _convertersList.Add(converter.Types[0].Name, converter);
        }

        public static XNBContentConverter? GetConverterFor(TypeAssemblyQualifier typeName)
        {
            if (_convertersList.ContainsKey(typeName)) return _convertersList[typeName];
            else return null;
        }

        public static XNBContentConverter? GetConverterFor(string filetype)
        {
            return _convertersList.Values.FirstOrDefault(c => c.FileFormat == filetype);
        }



        // reads content of XNB file and saves it to the file if converter exists
        public static bool TrySave(XNBFile file, string filepath)
        {
            XNBFile decompFile = file.Decompressed();
            if (!decompFile.IsValid) return false;

            using var xnbStream = new MemoryStream(decompFile.Content);
            using var xnbReader = new BinaryReader(xnbStream, Encoding.UTF8, false);

            int readerCount = xnbReader.Read7BitEncodedInt();

            List<TypeAssemblyQualifier> usedTypes = new List<TypeAssemblyQualifier>();
            List<int> typeReaderVersions = new List<int>();

            for (var i = 0; i < readerCount; i++)
            {
                string readerName = xnbReader.ReadString();
                int readerVersion = xnbReader.ReadInt32();

                var qualifier = new TypeAssemblyQualifier(readerName);

                usedTypes.Add(qualifier);
                typeReaderVersions.Add(readerVersion);
            }

            // main + shared resources count 
            int resourceCount = 1 + xnbReader.Read7BitEncodedInt();

            // FEZ XNB files shouldn't have more than 1 resource
            if (resourceCount != 1)
            {
                throw new InvalidDataException();
            }

            int resourceTypeID = xnbReader.Read7BitEncodedInt();
            TypeAssemblyQualifier mainType = usedTypes[resourceTypeID - 1];
            int mainTypeVersion = typeReaderVersions[resourceTypeID - 1];

            Console.WriteLine($"====MAIN TYPE: {mainType}, Version: {mainTypeVersion}====");

            var converter = GetConverterFor(mainType);

            if (converter == null)
            {
                Console.WriteLine("UNKNOWN DATA FORMAT: " + mainType);
                return false;
            }

            // we're ready to save the file!
            string extension = converter.FileFormat;

            using var fileStream = new FileStream($"{filepath}.{extension}", FileMode.Create, FileAccess.Write);
            using var fileWriter = new BinaryWriter(fileStream, Encoding.UTF8, false);

            converter.FromBinary(xnbReader, fileWriter);

            return true;
        }

        public static bool TryLoad(string filepath, out XNBFile file)
        {
            file = new XNBFile();
            if (!File.Exists(filepath)) return false;

            // checking if we have a converter that can be used to read given file name
            string fileFormat = Path.GetExtension(filepath).Replace(".", "");

            var converter = GetConverterFor(fileFormat);
            if (converter == null) return false;

            // preparing data to read from later on
            using var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            using var fileReader = new BinaryReader(fileStream);

            // preparing data stream to write to
            using var xnbStream = new MemoryStream();
            using var xnbWriter = new BinaryWriter(xnbStream);

            // write all types into header
            xnbWriter.Write7BitEncodedInt(converter.Types.Length);
            foreach(var type in converter.Types)
            {
                xnbWriter.Write(type.Name.GetDisplayName(false));
                xnbWriter.Write(0);
            }

            // number of shared resources (0)
            xnbWriter.Write7BitEncodedInt(0);

            // main resource id (in my system, first one is always the primary one)
            xnbWriter.Write7BitEncodedInt(1);

            // convert actual data
            converter.ToBinary(fileReader, xnbWriter);

            // put data into the file
            byte[] data = new byte[xnbStream.Length];
            xnbStream.Position = 0;
            xnbStream.Read(data, 0, (int)xnbStream.Length);
            file.SetData(data);

            return true;
        }
    }
}
