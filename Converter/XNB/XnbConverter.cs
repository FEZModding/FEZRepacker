using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB
{
    public class XnbConverter
    {
        public XnbFormatConverter? FormatConverter { get; private set; }
        public XnbAssemblyQualifier FileType { get; private set; }
        public int FileTypeVersion { get; private set; }
        public bool HeaderValid { get; private set; }
        public bool Converted => FormatConverter != null;

        public XnbConverter()
        {
            FormatConverter = null;
        }

        public Stream Convert(Stream input)
        {
            Stream output = new MemoryStream();
            Stream decompressedInput = XnbCompressor.Decompress(input);

            if (!XnbHeader.TryRead(decompressedInput, out var header))
            {
                HeaderValid = false;
                decompressedInput.Position = 0;
                decompressedInput.CopyTo(output);
                output.Position = 0;
                return output;
            }
            HeaderValid = true;

            using var xnbReader = new BinaryReader(decompressedInput);

            int fileSize = xnbReader.ReadInt32();
            int readerCount = xnbReader.Read7BitEncodedInt();

            List<XnbAssemblyQualifier> usedTypes = new List<XnbAssemblyQualifier>();
            List<int> typeReaderVersions = new List<int>();

            for (var i = 0; i < readerCount; i++)
            {
                string readerName = xnbReader.ReadString();
                int readerVersion = xnbReader.ReadInt32();

                var qualifier = new XnbAssemblyQualifier(readerName);

                usedTypes.Add(qualifier);
                typeReaderVersions.Add(readerVersion);
            }

            // main + shared resources count 
            int resourceCount = 1 + xnbReader.Read7BitEncodedInt();

            // FEZ XNB files shouldn't have more than 1 resource
            if (resourceCount != 1)
            {
                throw new InvalidDataException("XNB file doesn't contain exactly 1 resource.");
            }

            int resourceTypeID = xnbReader.Read7BitEncodedInt();
            FileType = usedTypes[resourceTypeID - 1];
            FileTypeVersion = typeReaderVersions[resourceTypeID - 1];

            FormatConverter = XnbFormatList.FindByQualifier(FileType);

            if (FormatConverter == null)
            {
                // we cannot convert this XNB file - just save the decompressed one
                decompressedInput.Position = 0;
                decompressedInput.CopyTo(output);
                output.Position = 0;
                return output;
            }

            var fileWriter = new BinaryWriter(output);

            FormatConverter.FromBinary(xnbReader, fileWriter);
            output.Position = 0;
            return output;
        }
    }
}