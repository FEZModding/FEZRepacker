using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types
{
    public abstract class XnbContentType
    {
        public XnbFormatConverter Converter { get; private set; }
        public abstract XnbAssemblyQualifier Name { get; }
        public abstract Type BasicType { get; }

        public XnbContentType(XnbFormatConverter converter)
        {
            Converter = converter;
        }

        public abstract object Read(BinaryReader reader);

        public abstract void Write(object data, BinaryWriter writer);

        public virtual bool IsEmpty(object data)
        {
            return false;
        }
    }

    // a little helper generalized class, so I don't have to override BasicType
    internal abstract class XnbContentType<T> : XnbContentType
    {
        public XnbContentType(XnbFormatConverter converter) : base(converter){}

        public override Type BasicType => typeof(T);
    }
}
