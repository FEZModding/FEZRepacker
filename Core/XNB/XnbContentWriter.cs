using System.Text;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Extension of BinaryWriter that allows writing XNB content to streams,
    /// using individual type serializers defined within provided <see cref="XnbPrimaryContentIdentity"/>.
    /// </summary>
    internal class XnbContentWriter : BinaryWriter
    {
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

            int typeId = Identity.ContentTypes.FindIndex(t => t.ContentType == T);
            if (typeId < 0)
            {
                throw new XnbSerializationException($"Couldn't assign type for {data} in {GetType()}");
            }
            
            if (!skipIdentifier)
            {
                int publicTypeId = Identity.PublicContentTypes.FindIndex(t => t.ContentType == T);
                if (publicTypeId < 0)
                {
                    throw new XnbSerializationException(
                        $"Attempted to write index of private content type {Identity.ContentTypes[typeId].Name}");
                }
                Write7BitEncodedInt(publicTypeId + 1);
            }
            Identity.ContentTypes[typeId].Serialize(data, this);
        }
    }
}
