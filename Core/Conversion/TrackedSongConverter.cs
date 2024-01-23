
using FEZRepacker.Core.Definitions.Game.TrackedSong;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.Helpers.Json;

namespace FEZRepacker.Core.Conversion
{
    internal class TrackedSongConverter : FormatConverter<TrackedSong>
    {
        public override string FileFormat => ".fezsong";

        public override FileBundle ConvertTyped(TrackedSong data)
        {
            return ConfiguredJsonSerializer.SerializeToFileBundle(FileFormat, data);
        }

        public override TrackedSong DeconvertTyped(FileBundle bundle)
        {
            return ConfiguredJsonSerializer.DeserializeFromFileBundle<TrackedSong>(bundle);
        }
    }
}
