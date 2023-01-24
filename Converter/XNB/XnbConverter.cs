using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB
{
    public class XnbConverter
    {
        public XnbFormatConverter? FormatConverter;
        public bool Converted { get; private set; }

        public XnbConverter()
        {
            FormatConverter = null;
            Converted = false;
        }

        public void Convert(Stream input, Stream output)
        {
            Stream decompressedInput = XnbCompressor.Decompress(input);

            if(!XnbHeader.TryRead(decompressedInput, out var header))
            {
                Converted = false;
                decompressedInput.CopyTo(output);
                return;
            }

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
            XnbAssemblyQualifier mainType = usedTypes[resourceTypeID - 1];
            int mainTypeVersion = typeReaderVersions[resourceTypeID - 1];

            FormatConverter = XnbFormatList.FindByQualifier(mainType);

            if (FormatConverter == null)
            {
                // we cannot convert this XNB file - just save the decompressed one
                Converted = false;
                decompressedInput.CopyTo(output);
                return;
            }

            using var fileWriter = new BinaryWriter(output);

            FormatConverter.FromBinary(xnbReader, fileWriter);

            Converted = true;
        }
    }
}
