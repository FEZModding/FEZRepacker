namespace FEZRepacker.XNB
{
    abstract class XNBContentConverter
    {
        public abstract XNBContentType[] Types { get; }
        public XNBContentType? PrimaryType => Types[0];
        public abstract string FileFormat { get; }

        public abstract void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter);
        public abstract void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter);

        protected virtual void ValidateType()
        {
            if (PrimaryType == null)
            {
                throw new InvalidDataException($"{this.GetType().Name} doesn't have primary type defined.");
            }
        }

        public XNBContentConverter()
        {
            ValidateType();
        }

        public T? ReadType<T>(BinaryReader reader)
        {
            int type = reader.Read7BitEncodedInt();
            if (type > 0 && type <= Types.Length)
            {
                var typeConverter = Types[type - 1];
                if (typeConverter.BasicType == typeof(T))
                {
                    return (T)typeConverter.Read(reader);
                }
                else
                {
                    throw new InvalidDataException($"Tried to read {typeof(T).Name}, found {typeConverter.BasicType} instead.");
                }
            }
            // type is either null or invalid
            return default(T);
        }

        public void WriteType<T>(T data, BinaryWriter writer)
        {
            XNBContentType? type = Types.ToList().Find(t => t.BasicType == typeof(T));
            if (type != null && data != null)
            {
                type.Write(data, writer);
            }
            else
            {
                writer.Write((byte)0x00);
            }
        }
    }
}
