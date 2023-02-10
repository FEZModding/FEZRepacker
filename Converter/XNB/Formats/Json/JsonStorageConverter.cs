using System.Text;

using FEZRepacker.Converter.FileSystem;

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

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            T data = (T)PrimaryContentType.Read(xnbReader);

            var json = CustomJsonSerializer.Serialize(data);

            var outStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return FileBundle.Single(outStream, FileFormat);
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            using var inReader = new BinaryReader(bundle.GetData(), Encoding.UTF8, true);
            string json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            T data = CustomJsonSerializer.Deserialize<T>(json);

            PrimaryContentType.Write(data, xnbWriter);
        }
    }
}
