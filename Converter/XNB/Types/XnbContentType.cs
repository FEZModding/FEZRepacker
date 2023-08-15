using FEZRepacker.Converter.Definitions;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB.Types
{
    /// <summary>
    /// Gives basic information and read/write methods for specific asset types in XNB files.
    /// </summary>
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
        /// <summary>
        /// Uses given binary reader to read an object of implemented content type.
        /// </summary>
        /// <param name="reader">Binary reader to read an object from.</param>
        /// <returns>Object of type defined in this content type structure.</returns>
        public abstract object Read(BinaryReader reader);

        /// <summary>
        /// Writes given object into a given binary writer.
        /// </summary>
        /// <param name="data">Object to write, preferably of the type defined in this content type structure.</param>
        /// <param name="writer">Binary writer to write an object to.</param>
        public abstract void Write(object data, BinaryWriter writer);

        /// <summary>
        /// Used to determine whether an object of this content type is empty.
        /// Used by a converter to read/write nullable types.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

    /// <summary>
    /// Helper class for <c>XnbContentType</c> to automatically assign <c>BasicType</c> based on given template.
    /// </summary>
    /// <typeparam name="T">Type to use for <c>BasicType</c> field.</typeparam>
    internal abstract class XnbContentType<T> : XnbContentType
    {
        public XnbContentType(XnbFormatConverter converter) : base(converter) { }

        public override Type BasicType => typeof(T);
    }
}
