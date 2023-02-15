using System.Text;

using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.XNB.Formats.Json;
using FEZRepacker.Converter.XNB.Types;
using FEZRepacker.Converter.XNB.Types.System;

namespace FEZRepacker.Converter.XNB.Formats
{
    internal class TrackedSongConverter : JsonStorageConverter<TrackedSong>
    {
        public override List<XnbContentType> TypesFactory => new List<XnbContentType>
        {
            new GenericContentType<TrackedSong>(this),
            new ListContentType<Loop>(this),
            new GenericContentType<Loop>(this),
            new ArrayContentType<ShardNotes>(this),
            new EnumContentType<ShardNotes>(this),
            new Int32ContentType(this),
            new EnumContentType<AssembleChords>(this)
        };

        public override string FileFormat => ".fezsong";
    }
}
