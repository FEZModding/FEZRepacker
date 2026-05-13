using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.Definitions.Game.TrackedSong;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.Definitions.Json;

namespace FEZRepacker.Core.Helpers.Json
{
    internal static class JsonContexts
    {
        public static List<JsonSerializerContext> List =>
        [
            ArtObjectJsonSerializerContext.Default,
            LevelJsonSerializerContext.Default,
            MapTreeJsonSerializerContext.Default,
            NpcMetadataJsonSerializerContext.Default,
            SkyJsonSerializerContext.Default,
            SpriteFontJsonSerializerContext.Default,
            TextStorageJsonSerializerContext.Default,
            TrackedSongJsonSerializerContext.Default,
            TrileSetJsonSerializerContext.Default
        ];
    }
    
    [JsonSerializable(typeof(ArtObject))]
    [JsonSerializable(typeof(IndexedPrimitives<VertexInstance, Matrix>))]
    internal partial class ArtObjectJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(LevelJsonModel))]
    internal partial class LevelJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(MapTreeJsonModel))]
    internal partial class MapTreeJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(NpcMetadata))]
    internal partial class NpcMetadataJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(Sky))]
    internal partial class SkyJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(SpriteFontPropertiesJsonModel))]
    internal partial class SpriteFontJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(OrderedDictionary<string, OrderedDictionary<string, string>>))]
    internal partial class TextStorageJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(TrackedSong))]
    internal partial class TrackedSongJsonSerializerContext : JsonSerializerContext {}
    
    [JsonSerializable(typeof(TrileSet))]
    [JsonSerializable(typeof(IndexedPrimitives<VertexInstance, Vector4>))]
    
    internal partial class TrileSetJsonSerializerContext : JsonSerializerContext {}
}
