using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Types;

namespace FEZRepacker.Converter.XNB.Formats
{
    /// <summary>
    /// Defines information and methods for converting XNB assets into file bundles and vice-versa.
    /// Each format converter needs to define methods of conversion, a main file bundle format
    /// and a list of content types this converter deals with.
    /// </summary>
    public abstract class XnbFormatConverter
    {
        public List<XnbContentType> ContentTypes { get; private set; }
        public List<XnbContentType> PublicContentTypes { get; private set; }
        public abstract List<XnbContentType> TypesFactory { get; }
        public XnbContentType PrimaryContentType => PublicContentTypes[0];
        public string FormatName => PrimaryContentType.Name.Name.Replace("Reader", "");
        public abstract string FileFormat { get; }

        public XnbFormatConverter()
        {
            ContentTypes = TypesFactory;
            PublicContentTypes = ContentTypes.Where(t => !t.IsPrivate).ToList();
            ValidateType();
        }

        public abstract FileBundle ReadXNBContent(BinaryReader xnbReader);
        public abstract void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter);

        protected virtual void ValidateType()
        {
            if (ContentTypes.Count == 0)
            {
                throw new InvalidDataException($"{this.GetType().Name} doesn't have primary type defined.");
            }
        }

        public T? ReadType<T>(BinaryReader reader, bool skipIdentifier = false)
        {
            object? read = ReadType(typeof(T), reader, skipIdentifier);
            return read != null ? (T)read : default(T);
        }

        public object? ReadType(Type T, BinaryReader reader, bool skipIdentifier = false)
        {
            if (T.IsPrimitive) skipIdentifier = true;

            int type = skipIdentifier ? 0 : reader.Read7BitEncodedInt();
            // if (type > 0 && type <= Types.Length)
            // {
            //     var typeConverter = Types[type - 1];
            //     if (typeConverter.BasicType == T)
            //     {
            //         return typeConverter.Read(reader);
            //     }
            //     else
            //     {
            //         throw new InvalidDataException($"Tried to read {T.Name}, found {typeConverter.BasicType} instead.");
            //     }
            // }

            // since types used in different files may vary, we're not using
            // the lookup table. instead, just find the reader we need.

            if (type > 0 || skipIdentifier)
            {
                XnbContentType? typeConverter = ContentTypes.ToList().Find(t => t.BasicType == T);
                if (typeConverter != null)
                {
                    return typeConverter.Read(reader);
                }
                else
                {
                    throw new InvalidDataException($"Cannot convert value of type {T.FullName}");
                }
            }

            return null;
        }

        public void WriteType<T>(T data, BinaryWriter writer, bool skipIdentifier = false)
        {
            WriteType(typeof(T), data, writer, skipIdentifier);
        }

        public void WriteType(Type T, object? data, BinaryWriter writer, bool skipIdentifier = false)
        {
            if (T.IsPrimitive) skipIdentifier = true;

            int typeID = ContentTypes.FindIndex(t => t.BasicType == T);
            if (typeID >= 0 && data != null)
            {
                if (!skipIdentifier && ContentTypes[typeID].IsEmpty(data))
                {
                    writer.Write((byte)0x00);
                }
                else
                {
                    if (!skipIdentifier)
                    {
                        int publicTypeID = PublicContentTypes.FindIndex(t => t.BasicType == T);
                        if (publicTypeID < 0)
                        {
                            throw new InvalidOperationException($"Attempted to write index of private content type {ContentTypes[typeID].Name}");
                        }
                        writer.Write7BitEncodedInt(publicTypeID + 1);
                    }
                    ContentTypes[typeID].Write(data, writer);
                }
            }
            else
            {
                if (!skipIdentifier)
                {
                    Console.WriteLine($"WARNING! Couldn't assign type for {data} in {this.GetType()}");
                    writer.Write((byte)0x00);
                }
            }
        }
    }
}
