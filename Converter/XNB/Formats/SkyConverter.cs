using System.Text;

using FEZRepacker.Converter.Definitions.FezEngine;
using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.Definitions.FezEngine.Structure.Input;
using FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting;
using FEZRepacker.Converter.FileSystem;
using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Formats.Json.CustomStructures;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

using static FEZRepacker.Converter.XNB.Formats.Json.CustomStructures.ModifiedMapTree;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class SkyConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<Sky>(this),
            new ListContentType<SkyLayer>(this),
            new GenericContentType<SkyLayer>(this),
            new ListContentType<string>(this),
            new StringContentType(this)
        };

        public override string FileFormat => ".fezsky";

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            XnbContentType primaryType = PrimaryContentType;

            Sky data = (Sky)primaryType.Read(xnbReader);

            var json = CustomJsonSerializer.Serialize(data);

            var outStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return FileBundle.Single(outStream, FileFormat);
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            var inReader = new BinaryReader(bundle.GetData());
            string json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            var nodeDict = CustomJsonSerializer.Deserialize<Sky>(json);

            PrimaryContentType.Write(nodeDict, xnbWriter);
        }
    }
}
