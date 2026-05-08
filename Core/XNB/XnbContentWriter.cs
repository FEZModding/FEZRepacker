using System.Text;

using FEZRepacker.Core.Helpers;
using FEZRepacker.Core.XNB.ContentSerialization;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Extension of BinaryWriter that allows writing XNB content to streams,
    /// using individual type serializers defined within provided <see cref="XnbPrimaryContentIdentity"/>.
    /// </summary>
    internal class XnbContentWriter : BinaryWriter
    {
        private readonly List<XnbContentSerializer> _claimedContentTypes = new();
        
        public readonly XnbPrimaryContentIdentity Identity;

        public XnbContentWriter(Stream stream, XnbPrimaryContentIdentity identity, bool leaveOpen = false) 
            : base(stream, Encoding.UTF8, leaveOpen)
        {
            Identity = identity;
        }

        public void WriteContent<T>(T data, bool skipIdentifier = false)
        {
            WriteContent(typeof(T), data, skipIdentifier);
        }

        public void WriteContent(Type T, object? data, bool skipIdentifier = false)
        {
            if (T.IsPrimitive) skipIdentifier = true;

            if (data == null)
            {
                Write((byte)0x00);
                return;
            }

            var matchingSerializer = Identity.ContentSerializers.Find(t => t.ContentType == T);
            if (matchingSerializer == null)
            {
                throw new XnbSerializationException($"Couldn't assign type for {data} in {GetType()}");
            }

            var typeIdentifier = TryClaimContentSerializer(matchingSerializer);
            
            if (!skipIdentifier)
            {
                if (typeIdentifier < 0)
                {
                    throw new XnbSerializationException($"Failed to index required content type {matchingSerializer.Name}");
                }
                Write7BitEncodedInt(typeIdentifier + 1);
            }
            
            matchingSerializer.Serialize(data, this);
        }

        private int TryClaimContentType(Type type)
        {
            var matchingSerializer = Identity.ContentSerializers.Find(t => t.ContentType == type);
            if (matchingSerializer == null)
            {
                return -1;
            }
            
            return TryClaimContentSerializer(matchingSerializer);
        }
        
        private int TryClaimContentSerializer(XnbContentSerializer serializer)
        {
            if (serializer.IsPrivate)
            {
                return -1;
            }
            
            var alreadyClaimedIndex = _claimedContentTypes.IndexOf(serializer);
            if (alreadyClaimedIndex >= 0)
            {
                return alreadyClaimedIndex;
            }
            
            var claimedIndex = _claimedContentTypes.Count;
            _claimedContentTypes.Add(serializer);
            ClaimGenericContentTypes(serializer.ContentType);
            return claimedIndex;
        }

        private void ClaimGenericContentTypes(Type type)
        {
            if (type.IsGenericType)
            {
                foreach (Type genericType in type.GetGenericArguments())
                {
                    TryClaimContentType(genericType);
                }
            }

            if (type.HasElementType)
            {
                TryClaimContentType(type.GetElementType()!);
            }
        }

        public byte[] ProduceAssemblyQualifiersListData()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
            
            writer.Write7BitEncodedInt(_claimedContentTypes.Count);
            foreach (var type in _claimedContentTypes)
            {
                writer.Write(type.Name.GetDisplayName(false)); // name
                writer.Write(0); // version (always 0 in original game assets)
            }
            
            return stream.ToArray();
        }
    }
}
