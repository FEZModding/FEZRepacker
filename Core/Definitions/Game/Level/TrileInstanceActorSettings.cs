namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.InstanceActorSettings")]
    [XnbReaderType("FezEngine.Readers.InstanceActorSettingsReader")]
    // Original name in FezEngine: InstanceActorSettings
    public class TrileInstanceActorSettings 
    {
        [XnbProperty(Optional = true)]
        public int? ContainedTrile { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SignText { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public bool[] Sequence { get; set; } = { };

        [XnbProperty(UseConverter = true)]
        public string SequenceSampleName { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string SequenceAlternateSampleName { get; set; } = "";

        [XnbProperty(Optional = true)]
        public int? HostVolume { get; set; }
    }
}
