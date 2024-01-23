using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.XNA;


namespace FEZRepacker.Core.Definitions.Game.TrileSet
{
    [XnbType("FezEngine.Structure.TrileSet")]
    [XnbReaderType("FezEngine.Readers.TrileSetReader")]
    public class TrileSet
    {
        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, Trile> Triles { get; set; }

        [JsonIgnore]
        [XnbProperty(UseConverter = true)]
        public Texture2D TextureAtlas { get; set; }


        public TrileSet()
        {
            TextureAtlas = new Texture2D();
            Triles = new Dictionary<int, Trile>();
        }
    }
}
