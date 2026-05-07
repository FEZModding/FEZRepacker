namespace FEZRepacker.Core.XNB.ContentSerialization
{
    /// <summary>
    /// Gives information and read/write methods for specific content types in XNB files.
    /// </summary>
    internal abstract class XnbContentSerializer
    {
        public abstract XnbAssemblyQualifier Name { get; }
        public abstract Type ContentType { get; }
        public bool IsPrivate { get; protected set; }

        public XnbContentSerializer()
        {
            IsPrivate = false;
        }
        /// <summary>
        /// Uses given binary reader to read an object of implemented content type.
        /// </summary>
        /// <param name="serializer">Binary reader to read an object from.</param>
        /// <returns>Object of type defined in this content type structure.</returns>
        public abstract object Deserialize(XnbContentReader reader);

        /// <summary>
        /// Writes given object into a given binary writer.
        /// </summary>
        /// <param name="data">Object to write, preferably of the type defined in this content type structure.</param>
        /// <param name="writer">Binary writer to write an object to.</param>
        public abstract void Serialize(object data, XnbContentWriter writer);

        /// <summary>
        /// Marks content serializer private - its qualifier name won't appear in XNB header.
        /// </summary>
        public XnbContentSerializer MarkPrivate()
        {
            IsPrivate = true;
            return this;
        }
    }

    /// <summary>
    /// Helper class for <c>XnbContentType</c> to automatically assign <c>BasicType</c> based on given template.
    /// </summary>
    /// <typeparam name="T">Type to use for <c>BasicType</c> field.</typeparam>
    internal abstract class XnbContentSerializer<T> : XnbContentSerializer
    {
        public override Type ContentType => typeof(T);
    }
}
