using FEZRepacker.XNB.Attributes;

namespace FEZRepacker.Definitions.FezEngine.Structure
{
    [XNBType("FezEngine.Readers.InstanceActorSettingsReader")]
    public class TrileInstanceActorSettings // Original name in FezEngine: InstanceActorSettings
    {
        [XNBProperty(Optional = true)]
        public int? ContainedTrile { get; set; }

        [XNBProperty(UseConverter = true)]
        public string SignText { get; set; }

        [XNBProperty(UseConverter = true)]
        public bool[] Sequence { get; set; }

        [XNBProperty(UseConverter = true)]
        public string SequenceSampleName { get; set; }

        [XNBProperty(UseConverter = true)]
        public string SequenceAlternateSampleName { get; set; }

        [XNBProperty(Optional = true)]
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
