using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FEZRepacker.XNB.Converters.Files;

namespace FEZRepacker.XNB
{
    static class XNBContentConvert
    {
        private static Dictionary<TypeAssemblyQualifier, XNBContentConverter> _convertersList = new();
        static XNBContentConvert()
        {
            AddConverter(new TextStorageConverter());
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



        // reads content of XNB file and saves it to the file if converter exists
        public static bool TrySave(XNBFile file, string filepath)
        {
            XNBFile decompFile = file.Decompressed();
            if (!decompFile.IsValid) return false;

            using var xnbStream = new MemoryStream(decompFile.Content);
            using var xnbReader = new BinaryReader(xnbStream, Encoding.UTF8, false);

            int readerCount = xnbReader.Read7BitEncodedInt();

            List<TypeAssemblyQualifier> usedTypes = new List<TypeAssemblyQualifier>();

            for (var i = 0; i < readerCount; i++)
            {
                string readerName = xnbReader.ReadString();
                int readerVersion = xnbReader.ReadInt32();

                var qualifier = new TypeAssemblyQualifier(readerName);

                usedTypes.Add(qualifier);
            }

            // shared resource count + main resource
            int resourceCount = xnbReader.Read7BitEncodedInt() + 1;

            // FEZ XNB files shouldn't have more than 1 resource
            if (resourceCount != 1)
            {
                throw new InvalidDataException();
            }

            int resourceTypeID = xnbReader.Read7BitEncodedInt();
            TypeAssemblyQualifier mainType = usedTypes[resourceTypeID - 1];


            Console.WriteLine("====MAIN TYPE:" + mainType + "====");

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
    }
}
