
using System.Text;

using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentTypes;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Extension of BinaryReader that allows reading XNB content from streams,
    /// using individual type serializers defined within provided <see cref="XnbPrimaryContentIdentity"/>.
    /// </summary>
    internal class XnbContentReader : BinaryReader
    {
        public readonly XnbPrimaryContentIdentity Identity;
        public XnbContentReader(Stream input, XnbPrimaryContentIdentity identity, bool leaveOpen = true) 
            : base(input, Encoding.UTF8, leaveOpen) {
            Identity = identity;
        }

        public T? ReadContent<T>(bool skipIdentifier = false)
        {
            object? read = ReadContent(typeof(T), skipIdentifier);
            return read != null ? (T)read : default;
        }

        public object? ReadContent(Type T, bool skipIdentifier = false)
        {
            if (!skipIdentifier && !T.IsPrimitive)
            {
                int type = Read7BitEncodedInt();
                if (type == 0)
                {
                    return null;
                }
                // note: we should technically reference assembly qualifiers array using this index
                // to determine what serializer to use, but since we know what type is expected of us
                // and there's only one reader per type, this is redundant.
            }

            XnbContentSerializer? serializer = Identity.ContentSerializers.Find(t => t.ContentType == T);
            if (serializer == null)
            {
                throw new InvalidDataException($"Cannot convert value of type {T.FullName}");
            }
            
            return serializer.Deserialize(this);
        }
    }
}
