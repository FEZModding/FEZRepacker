using FEZRepacker.Converter.XNB.Types;

namespace FEZRepacker.Converter.XNB.Formats
{
    public abstract class XnbFormatConverter
    {
        public XnbContentType[] ContentTypes { get; private set; }
        public abstract XnbContentType[] TypesFactory { get; }
        public XnbContentType PrimaryContentType => ContentTypes[0];
        public string FormatName => PrimaryContentType.Name.Name.Replace("Reader", "");
        public abstract string FileFormat { get; }

        public abstract void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter);
        public abstract void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter);

        protected virtual void ValidateType()
        {
            if (PrimaryContentType == null)
            {
                throw new InvalidDataException($"{this.GetType().Name} doesn't have primary type defined.");
            }
        }

        public XnbFormatConverter()
        {
            ContentTypes = TypesFactory;
            ValidateType();
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
                    throw new InvalidDataException($"Cannot convert value of type {T.Name}");
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

            int typeID = ContentTypes.ToList().FindIndex(t => t.BasicType == T);
            if (typeID >= 0 && data != null)
            {
                if(!skipIdentifier && ContentTypes[typeID].IsEmpty(data))
                {
                    writer.Write((byte)0x00);
                }
                else
                {
                    if (!skipIdentifier)
                    {
                        writer.Write7BitEncodedInt(typeID + 1);
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
