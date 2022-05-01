using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FEZRepacker.XNB.Converters
{
    class TextStorageContent : XNBContent
    {
        public Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();
        public override XNBContentConverter Converter => TextStorageContentConverter.Instance;
    }

    class TextStorageContentConverter : XNBContentConverter
    {
        public static readonly TextStorageContentConverter Instance = new TextStorageContentConverter();

        public override TypeAssemblyQualifier[] DataTypes => new TypeAssemblyQualifier[] {
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.String],[System.Collections.Generic.Dictionary`2[[System.String],[System.String]]]]"),
            new("Microsoft.Xna.Framework.Content.StringReader"),
            new("Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.String],[System.String]]")
        };

        public override string FileFormat => "fezdata";

        public override XNBContent Read(BinaryReader reader)
        {
            TextStorageContent data = new TextStorageContent();

            int dataCount = reader.ReadInt32();

            for (int i = 0; i < dataCount; i++)
            {
                reader.ReadByte(); // 0x02
                string key = reader.ReadString();

                Dictionary<string, string> value = new Dictionary<string, string>();

                reader.ReadByte(); // 0x03
                int valueDataCount = reader.ReadInt32();

                for(int j = 0; j < valueDataCount; j++)
                {
                    reader.ReadByte(); // 0x02
                    string valueKey = reader.ReadString();
                    reader.ReadByte(); // 0x02
                    string valueValue = reader.ReadString();
                    value.Add(valueKey, valueValue);
                }
                data.Data.Add(key, value);
            }

            return data;
        }

        public override void Write(XNBContent data, BinaryWriter writer)
        {
            if (!(data is TextStorageContent)) throw new InvalidDataException();
            TextStorageContent tsData = (TextStorageContent)data;

            writer.Write(tsData.Data.Count);

            foreach((var key, var value) in tsData.Data)
            {
                writer.Write((byte)0x02);
                writer.Write(key);

                writer.Write((byte)0x03);
                writer.Write(value.Count);
                foreach((var valueKey, var valueValue) in value)
                {
                    writer.Write((byte)0x02);
                    writer.Write(valueKey);

                    writer.Write((byte)0x02);
                    writer.Write(valueValue);
                }
            }
        }


        public override XNBContent ReadUnpacked(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteUnpacked(XNBContent data, BinaryWriter writer)
        {
            if (!(data is TextStorageContent)) throw new InvalidDataException();
            TextStorageContent tsData = (TextStorageContent)data;

            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yaml = serializer.Serialize(tsData.Data);

            writer.Write(Encoding.UTF8.GetBytes(yaml));
        }
    }
}
