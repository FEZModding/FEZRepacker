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
            var primaryContentType = ExtractPrimaryContentIdentity(decompressedInput);
            using var xnbReader = new XnbContentReader(decompressedInput, primaryContentType, true);
            return xnbReader.ReadContent(primaryContentType.PrimaryContentSerializer.ContentType, true);
        }

        /// <summary>
        /// Decompresses and deserializes XNB asset only enough to determine primary content type.
        /// Useful for efficiently determining a type of asset from XNB stream.
        /// </summary>
        /// <param name="xnbStream">A stream containing serialized XNB asset</param>
        /// <returns>A type assigned as a primary content type of given XNB asset.</returns>
        /// <exception cref="XnbSerializationException">
        /// Thrown when the stream does not contain a valid XNB file.
        /// </exception>
        public static Type DeserializePrimaryContentTypeOnly(Stream xnbStream)
        {
            using Stream decompressedInput = XnbCompressor.Decompress(xnbStream);
            var primaryContentType = ExtractPrimaryContentIdentity(decompressedInput);
            return primaryContentType.PrimaryContentSerializer.ContentType;
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

            using var xnbContentWriter = new XnbContentWriter(new MemoryStream(), contentIdentity, true);
            xnbContentWriter.Write7BitEncodedInt(0); // number of shared resources (0 for FEZ assets)
            xnbContentWriter.Write7BitEncodedInt(1); // main resource id (first one is always the primary one)
            xnbContentWriter.WriteContent(contentIdentity.PrimaryContentSerializer.ContentType, obj, true);

            var assemblyQualifiers = xnbContentWriter.ProduceAssemblyQualifiersListData();
            
            var fileLength = XnbHeader.Size + assemblyQualifiers.Length + (int)xnbContentWriter.BaseStream.Length;
            
            var outputStream = new MemoryStream();
            using var outputWriter = new BinaryWriter(outputStream, Encoding.UTF8, true);
            
            XnbHeader.Default.Write(outputStream);
            outputWriter.Write(fileLength);
            outputWriter.Write(assemblyQualifiers);
            xnbContentWriter.BaseStream.Position = 0;
            xnbContentWriter.BaseStream.CopyTo(outputStream);

            outputStream.Position = 0;
            return outputStream;
        }

        private static XnbPrimaryContentIdentity ExtractPrimaryContentIdentity(Stream xnbStream)
        {
            if (!XnbHeader.TryRead(xnbStream, out var header))
            {
                throw new XnbSerializationException("Unable to parse XNB header.");
            }

            if ((header.Flags & XnbHeader.XnbFlags.Compressed) != 0)
            {
                throw new XnbSerializationException("Cannot extract primary content identity from compressed XNB stream.");
            }

            using var binaryReader = new BinaryReader(xnbStream, Encoding.UTF8, true);

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
            
            return primaryContentType;
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
    }
}
