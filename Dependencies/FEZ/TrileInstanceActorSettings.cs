namespace FEZEngine.Structure
{
    public class TrileInstanceActorSettings // Original name in FezEngine: InstanceActorSettings
    {
        public int? ContainedTrile { get; set; }
        public string SignText { get; set; }
        public bool[] Sequence { get; set; }
        public string SequenceSampleName { get; set; }
        public string SequenceAlternateSampleName { get; set; }
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
