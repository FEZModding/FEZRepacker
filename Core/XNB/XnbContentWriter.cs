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

            int typeID = Identity.ContentTypes.FindIndex(t => t.ContentType == T);
            if (typeID >= 0 && data != null)
            {
                if (!skipIdentifier && Identity.ContentTypes[typeID].IsEmpty(data))
                {
                    Write((byte)0x00);
                }
                else
                {
                    if (!skipIdentifier)
                    {
                        int publicTypeID = Identity.PublicContentTypes.FindIndex(t => t.ContentType == T);
                        if (publicTypeID < 0)
                        {
                            throw new InvalidOperationException($"Attempted to write index of private content type {Identity.ContentTypes[typeID].Name}");
                        }
                        Write7BitEncodedInt(publicTypeID + 1);
                    }
                    Identity.ContentTypes[typeID].Serialize(data, this);
                }
            }
            else
            {
                if (!skipIdentifier)
                {
                    Console.WriteLine($"WARNING! Couldn't assign type for {data} in {GetType()}");
                    Write((byte)0x00);
                }
            }
        }
    }
}
