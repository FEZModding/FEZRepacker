using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.NpcMetadata
{
    [XnbType("FezEngine.Structure.NpcMetadata, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.NpcMetadataReader, FezEngine")]
    public class NpcMetadata
    {
        [XnbProperty]
        public float WalkSpeed { get; set; }

        [XnbProperty]
        public bool AvoidsGomez { get;set; }

        [XnbProperty(UseConverter = true)]
        public string SoundPath { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public List<NpcAction> SoundActions { get; set; } = new();
    }
}
