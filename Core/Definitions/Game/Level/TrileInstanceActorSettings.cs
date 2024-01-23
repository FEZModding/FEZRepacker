namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.InstanceActorSettings")]
    [XnbReaderType("FezEngine.Readers.InstanceActorSettingsReader")]
    internal class TrileInstanceActorSettings // Original name in FezEngine: InstanceActorSettings
    {
        [XnbProperty(Optional = true)]
        public int? ContainedTrile { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SignText { get; set; }

        [XnbProperty(UseConverter = true)]
        public bool[] Sequence { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SequenceSampleName { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SequenceAlternateSampleName { get; set; }

        [XnbProperty(Optional = true)]
        public int? HostVolume { get; set; }


        public TrileInstanceActorSettings()
        {
            SignText = "";
            Sequence = new bool[0];
            SequenceSampleName = "";
            SequenceAlternateSampleName = "";
        }
    }
}
