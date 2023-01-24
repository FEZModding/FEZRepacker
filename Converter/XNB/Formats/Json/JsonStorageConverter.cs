using System.Text;

namespace FEZRepacker.Converter.XNB.Formats.Json
{
    internal abstract class JsonStorageConverter<T> : XnbFormatConverter where T : notnull
    {
        protected override void ValidateType()
        {
            base.ValidateType();
            if (PrimaryContentType != null && PrimaryContentType.BasicType != typeof(T))
            {
                throw new InvalidDataException(
                    $"JsonStorageConverter uses type {typeof(T).Name}, while primary type is {PrimaryContentType.BasicType.Name}."
                );
            }
        }

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            T data = (T)PrimaryContentType.Read(xnbReader);

            var json = CustomJsonSerializer.Serialize(data);

            outWriter.Write(Encoding.UTF8.GetBytes(json));
            
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            string json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            T data = CustomJsonSerializer.Deserialize<T>(json);

            PrimaryContentType.Write(data, xnbWriter);
        }
    }
}
