using System.Text;

using FEZRepacker.Core.Helpers;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Main XNB serialization handling logic. Contains methods for deserializing
    /// XNB asset streams into objects and serializing objects into XNB streams.
    /// </summary>
    /// <remarks>
    /// Despite XNB files containing references to specific types within FEZ,
    /// this serializer is not using them, and instead is relying on Repacker's
    /// definitions, which can be found in "FEZRepacker.Core.Definitions".
    /// Additionally, asset will not be serialized nor deserialized if it's not
    /// one of the primary content types for FEZ, which are defined in 
    /// "FEZRepacker.Core.XNB.ContentTypes".
    /// </remarks>
    public static class XnbSerializer
    {
        /// <summary>
        /// Deserializes XNB asset encoded in given stream. Additionally, attempts
        /// to decompress it if a compressed XNB asset was given.
        /// </summary>
        /// <param name="xnbStream">A stream to deserialize XNB asset from</param>
        /// <returns>A reference to an object which was serialized within given stream.</returns>
        /// <exception cref="XnbSerializationException">
        /// Thrown when the stream does not contain a valid XNB file.
        /// </exception>
        public static object? Deserialize(Stream xnbStream)
        {
            using Stream decompressedInput = XnbCompressor.Decompress(xnbStream);

            if (!XnbHeader.TryRead(decompressedInput, out var header))
            {
                throw new XnbSerializationException("Unable to parse XNB header.");
            }

            using var binaryReader = new BinaryReader(decompressedInput, Encoding.UTF8, true);

            int fileSize = binaryReader.ReadInt32();

            var usedTypesQualifiers = ReadAssemblyQualifiersList(binaryReader);

            int additionalResourcesCount = binaryReader.Read7BitEncodedInt();

            // FEZ XNB files shouldn't have additional resources
            if (additionalResourcesCount > 0)
            {
                throw new XnbSerializationException("Additional XNB resources in a single file detected.");
            }

            int mainResourceTypeID = binaryReader.Read7BitEncodedInt();
            var mainResourceQualifier = usedTypesQualifiers[mainResourceTypeID - 1];

            var primaryContentType = XnbPrimaryContents.FindByQualifier(mainResourceQualifier);

            if (primaryContentType == null)
            {
                throw new XnbSerializationException($"Cannot find XNB primary format identity for type {mainResourceQualifier.Name}");
            }

            using var xnbReader = new XnbContentReader(decompressedInput, primaryContentType, true);
            return xnbReader.ReadContent(primaryContentType.PrimaryContentType.ContentType, true);
        }

        /// <summary>
        /// Produces a stream containing a given object, serialized into XNB format.
        /// </summary>
        /// <param name="obj">
        /// Object to serialize into an XNB asset. Must be a valid primary content type for FEZ.
        /// </param>
        /// <returns>The stream containing serialized data, positioned at the beginning.</returns>
        /// <exception cref="XnbSerializationException">
        /// Thrown when given object is not a valid primary content type for FEZ.
        /// </exception>
        public static Stream Serialize(object obj)
        {
            var contentIdentity = XnbPrimaryContents.FindByType(obj.GetType());

            if (contentIdentity == null)
            {
                throw new XnbSerializationException($"Cannot find XNB primary format identity for type {obj.GetType().Name}");
            }

            var outputStream = new MemoryStream();

            XnbHeader.Default.Write(outputStream);

            using var xnbContentWriter = new XnbContentWriter(new MemoryStream(), contentIdentity, true);

            WriteAssemblyQualifiersList(xnbContentWriter);

            xnbContentWriter.Write7BitEncodedInt(0); // number of shared resources (0 for FEZ assets)
            xnbContentWriter.Write7BitEncodedInt(1); // main resource id (first one is always the primary one)

            xnbContentWriter.WriteContent(contentIdentity.PrimaryContentType.ContentType, obj, true);

            // copy length of the file (including header block of 10 bytes) and data into output stream
            new BinaryWriter(outputStream).Write((int)xnbContentWriter.BaseStream.Length + 10);
            xnbContentWriter.BaseStream.Position = 0;
            xnbContentWriter.BaseStream.CopyTo(outputStream);

            outputStream.Position = 0;
            return outputStream;
        }


        private static List<XnbAssemblyQualifier> ReadAssemblyQualifiersList(BinaryReader xnbReader)
        {
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

            return usedTypes;
        }

        private static void WriteAssemblyQualifiersList(XnbContentWriter writer)
        {
            writer.Write7BitEncodedInt(writer.Identity.PublicContentTypes.Count);
            foreach (var type in writer.Identity.PublicContentTypes)
            {
                writer.Write(type.Name.GetDisplayName(false)); // name
                writer.Write(0); // version
            }
        }

    }
}
