namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.InstanceActorSettings, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.InstanceActorSettingsReader, FezEngine")]
    // Original name in FezEngine: InstanceActorSettings
    public class TrileInstanceActorSettings 
    {
        [XnbProperty(UseConverter = true)]
        public int? ContainedTrile { get; set; }

        [XnbProperty(UseConverter = true)]
        public string? SignText { get; set; }

        [XnbProperty(UseConverter = true)]
        public bool[]? Sequence { get; set; }

        [XnbProperty(UseConverter = true)]
        public string? SequenceSampleName { get; set; }

        [XnbProperty(UseConverter = true)]
        public string? SequenceAlternateSampleName { get; set; }

        [XnbProperty(UseConverter = true)]
        public int? HostVolume { get; set; }
    }
}
