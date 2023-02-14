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
    internal class MapTreeConverter : XnbFormatConverter
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<MapTree>(this),
            new GenericContentType<MapNode>(this),
            new ListContentType<MapNodeConnection>(this),
            new GenericContentType<MapNodeConnection>(this),
            new EnumContentType<FaceOrientation>(this),
            new Int32ContentType(this),
            new EnumContentType<LevelNodeType>(this),
            new GenericContentType<WinConditions>(this),
            new ListContentType<int>(this)
        };

        public override string FileFormat => ".fezmap";

        public override FileBundle ReadXNBContent(BinaryReader xnbReader)
        {
            XnbContentType primaryType = PrimaryContentType;

            MapTree data = (MapTree)primaryType.Read(xnbReader);

            var json = CustomJsonSerializer.Serialize(ModifiedMapTree.FromMapTree(data));

            var outStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return FileBundle.Single(outStream, FileFormat);
        }

        public override void WriteXnbContent(FileBundle bundle, BinaryWriter xnbWriter)
        {
            var inReader = new BinaryReader(bundle.GetData());
            string json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            var nodeDict = CustomJsonSerializer.Deserialize<Dictionary<int, ModifiedNode>>(json);

            var mapTree = ModifiedMapTree.ToMapTree(nodeDict);

            PrimaryContentType.Write(mapTree, xnbWriter);
        }
    }
}
