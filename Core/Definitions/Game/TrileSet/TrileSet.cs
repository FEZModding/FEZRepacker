using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.XNA;


namespace FEZRepacker.Core.Definitions.Game.TrileSet
{
    [XnbType("FezEngine.Structure.TrileSet, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.TrileSetReader, FezEngine")]
    public class TrileSet
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, Trile> Triles { get; set; } = new();

        [JsonIgnore]
        [XnbProperty(UseConverter = true)]
        public Texture2D TextureAtlas { get; set; } = new();
    }
}
