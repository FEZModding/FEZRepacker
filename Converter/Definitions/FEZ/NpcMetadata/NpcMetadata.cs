namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.NpcMetadata")]
    [XnbReaderType("FezEngine.Readers.NpcMetadataReader")]
    internal class NpcMetadata
    {
        [XnbProperty]
        public float WalkSpeed { get; set; }

        [XnbProperty]
        public bool AvoidsGomez { get;set; }

        [XnbProperty(UseConverter = true)]
        public string SoundPath { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<NpcAction> SoundActions { get; set; }


        public NpcMetadata()
        {
            SoundActions = new List<NpcAction>();
        }
    }
}
