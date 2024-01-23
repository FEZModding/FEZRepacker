
using System.Text;

using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentTypes;

namespace FEZRepacker.Core.XNB
{
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
            if (T.IsPrimitive) skipIdentifier = true;

            int type = skipIdentifier ? 0 : Read7BitEncodedInt();

            if (type > 0 || skipIdentifier)
            {
                XnbContentSerializer? typeConverter = Identity.ContentTypes.ToList().Find(t => t.ContentType == T);
                if (typeConverter != null)
                {
                    return typeConverter.Deserialize(this);
                }
                else
                {
                    throw new InvalidDataException($"Cannot convert value of type {T.FullName}");
                }
            }

            return null;
        }
    }
}
