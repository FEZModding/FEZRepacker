using System.Text;

using FEZRepacker.Conversion.Json;

namespace FEZRepacker.XNB.Converters
{
    abstract class JsonStorageConverter<T> : XNBContentConverter where T : notnull
    {
        protected override void ValidateType()
        {
            base.ValidateType();
            if (PrimaryType != null && PrimaryType.BasicType != typeof(T))
            {
                throw new InvalidDataException(
                    $"JsonStorageConverter uses type {typeof(T).Name}, while primary type is {Types[0].BasicType.Name}."
                );
            }
        }

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            XNBContentType primaryType = Types[0];
            
            T data = (T)primaryType.Read(xnbReader);

            var json = CustomJsonSerializer.Serialize(data);

            outWriter.Write(Encoding.UTF8.GetBytes(json));
            
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            string json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            T data = CustomJsonSerializer.Deserialize<T>(json);

            PrimaryType.Write(data, xnbWriter);
        }
    }
}
