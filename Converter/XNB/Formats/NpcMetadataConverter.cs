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
    internal class NpcMetadataConverter : JsonStorageConverter<NpcMetadata>
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<NpcMetadata>(this),
            new StringContentType(this),
            new ListContentType<NpcAction>(this, true),
            new EnumContentType<NpcAction>(this),
            new Int32ContentType(this)
        };

        public override string FileFormat => ".feznpc";
    }
}
