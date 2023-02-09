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

        private static void AppendGenericTypesToQualifier(Type type, ref XnbAssemblyQualifier qualifier)
        {
            if (!type.IsGenericType) return;

            var genericQualifiers = new List<XnbAssemblyQualifier>();

            foreach (Type genericType in type.GetGenericArguments())
            {
                var genericQualifier = GetXnbTypeFor(genericType);
                if (!genericQualifier.HasValue) continue;
                genericQualifiers.Add(genericQualifier.Value);
            }

            qualifier.Templates = genericQualifiers.ToArray();
        }

        protected static XnbAssemblyQualifier? GetXnbTypeFor(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(XnbTypeAttribute), false);
            if (attributes.Length > 0)
            {
                var qualifier = (attributes.First() as XnbTypeAttribute)!.Qualifier;
                AppendGenericTypesToQualifier(type, ref qualifier);
                return qualifier;
            }
            return null;
        }

        protected static XnbAssemblyQualifier? GetXnbReaderTypeFor(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(XnbReaderTypeAttribute), false);
            if (attributes.Length > 0)
            {
                var qualifier = (attributes.First() as XnbReaderTypeAttribute)!.Qualifier;
                AppendGenericTypesToQualifier(type, ref qualifier);
                return qualifier;
            }
            return null;
        }
    }

    // a little helper generalized class, so I don't have to override BasicType
    internal abstract class XnbContentType<T> : XnbContentType
    {
        public XnbContentType(XnbFormatConverter converter) : base(converter) { }

        public override Type BasicType => typeof(T);
    }
}
