namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.Loop")]
    [XnbReaderType("FezEngine.Readers.LoopReader")]
    internal class Loop
    {
        [XnbProperty]
        public int Duration { get; set; }

        [XnbProperty]
        public int LoopTimesFrom { get; set; }

        [XnbProperty]
        public int LoopTimesTo { get; set; }

        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty]
        public int TriggerFrom { get; set; }

        [XnbProperty]
        public int TriggerTo { get; set; }

        [XnbProperty]
        public int Delay { get; set; }

        [XnbProperty]
        public bool Night { get; set; }

        [XnbProperty]
        public bool Day { get; set; }

        [XnbProperty]
        public bool Dusk { get; set; }

        [XnbProperty]
        public bool Dawn { get; set; }

        [XnbProperty]
        public bool FractionalTime { get; set; }

        [XnbProperty]
        public bool OneAtATime { get; set; }

        [XnbProperty]
        public bool CutOffTail { get; set; }
    }
}
