using System.Text;

using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.XNB.ContentTypes;

namespace FEZRepacker.Core.XNB
{
    public static class XnbSerializer
    {
        public static object? Deserialize(Stream xnbStream)
        {
            Stream output = new MemoryStream();
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
                throw new XnbSerializationException("Additional XNB resources in a single file detected.");
            }

            using var xnbReader = new XnbContentReader(decompressedInput, primaryContentType, true);
            return xnbReader.ReadContent(primaryContentType.PrimaryContentType.ContentType, true);
        }



        public static Stream Serialize(object obj)
        {
            var contentIdentity = XnbPrimaryContents.FindByType(obj.GetType());

            if (contentIdentity == null)
            {
                throw new XnbSerializationException($"Cannot find XNB primary format identity for type {obj.GetType()}");
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
