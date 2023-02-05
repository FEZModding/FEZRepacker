using FEZRepacker.Converter.Definitions;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types
{
    public abstract class XnbContentType
    {
        public XnbFormatConverter Converter { get; private set; }
        public abstract XnbAssemblyQualifier Name { get; }
        public abstract Type BasicType { get; }
        public bool IsPrivate { get; protected set; }

        public XnbContentType(XnbFormatConverter converter)
        {
            Converter = converter;
            IsPrivate = false;
        }

        public abstract object Read(BinaryReader reader);

        public abstract void Write(object data, BinaryWriter writer);

        public virtual bool IsEmpty(object data)
        {
            return false;
        }

        protected static XnbAssemblyQualifier? GetXnbTypeFor(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(XnbTypeAttribute), false);
            if (attributes.Length > 0)
            {
                return (attributes.First() as XnbTypeAttribute)!.Qualifier;
            }
            return null;
        }

        protected static XnbAssemblyQualifier? GetXnbReaderTypeFor(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(XnbReaderTypeAttribute), false);
            if (attributes.Length > 0)
            {
                return (attributes.First() as XnbReaderTypeAttribute)!.Qualifier;
            }
            return null;
        }
    }

    // a little helper generalized class, so I don't have to override BasicType
    internal abstract class XnbContentType<T> : XnbContentType
    {
        public XnbContentType(XnbFormatConverter converter) : base(converter){}

        public override Type BasicType => typeof(T);
    }
}
