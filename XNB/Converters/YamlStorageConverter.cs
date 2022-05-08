using System.Text;

using FEZRepacker.Dependencies.Yaml;

namespace FEZRepacker.XNB.Converters
{
    abstract class YamlStorageConverter<T> : XNBContentConverter where T : notnull
    {
        protected override void ValidateType()
        {
            base.ValidateType();
            if (PrimaryType != null && PrimaryType.BasicType != typeof(T))
            {
                throw new InvalidDataException(
                    $"YamlStorageConverter uses type {typeof(T).Name}, while primary type is {Types[0].BasicType.Name}."
                );
            }
        }

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            XNBContentType primaryType = Types[0];
            
            T data = (T)primaryType.Read(xnbReader);

            var yaml = YamlSerializer.Serialize(data);

            outWriter.Write(Encoding.UTF8.GetBytes(yaml));
            
        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            string yaml = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            T data = YamlSerializer.Deserialize<T>(yaml);

            PrimaryType.Write(data, xnbWriter);
        }
    }
}
