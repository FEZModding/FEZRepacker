using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB
{
    /// <summary>
    /// Allows conversion of an XNB stream into a <c>FileBundle</c>.
    /// Uses primary asset type defined in an XNB file to find appropriate format converter,
    /// then uses found converter to create a FileBundle.
    /// </summary>
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

        /// <summary>
        /// Converts given stream into a <c>FileBundle</c>.
        /// </summary>
        /// <param name="input">Stream to convert.</param>
        /// <returns>
        /// <c>FileBundle</c> containing converted file. 
        /// If failed to read XNB asset, returns a copy of input stream as a file bundle.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// Thrown when more than 1 resource is present in XNB asset file, 
        /// which should not be the case for FEZ XNB assets.
        /// </exception>
        public FileBundle Convert(Stream input)
        {
            Stream output = new MemoryStream();
            using Stream decompressedInput = XnbCompressor.Decompress(input);

            if (!XnbHeader.TryRead(decompressedInput, out var header))
            {
                HeaderValid = false;
                decompressedInput.Position = 0;
                decompressedInput.CopyTo(output);
                output.Seek(0, SeekOrigin.Begin);

                return FileBundle.Single(output, ".xnb");
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
                output.Seek(0, SeekOrigin.Begin);
                return FileBundle.Single(output, ".xnb");
            }

            var convertedBundle = FormatConverter.ReadXNBContent(xnbReader);

            output.Dispose();

            return convertedBundle;
        }
    }
}
