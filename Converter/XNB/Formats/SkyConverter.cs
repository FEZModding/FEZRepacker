using System.Text;

using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

using static FEZRepacker.Converter.XNB.Formats.Json.CustomStructures.ModifiedMapTree;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class SkyConverter : JsonStorageConverter<Sky>
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
    }
}
