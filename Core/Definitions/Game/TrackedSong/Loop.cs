namespace FEZRepacker.Core.Definitions.Game.TrackedSong
{
    [XnbType("FezEngine.Structure.Loop, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.LoopReader, FezEngine")]
    public class Loop
    {
        [XnbProperty]
        public int Duration { get; set; } = 1;

        [XnbProperty]
        public int LoopTimesFrom { get; set; } = 1;

        [XnbProperty]
        public int LoopTimesTo { get; set; } = 1;

        [XnbProperty]
        public string? Name { get; set; } = "";

        [XnbProperty]
        public int TriggerFrom { get; set; }

        [XnbProperty]
        public int TriggerTo { get; set; }

        [XnbProperty]
        public int Delay { get; set; }

        [XnbProperty]
        public bool Night { get; set; } = true;

        [XnbProperty]
        public bool Day { get; set; } = true;

        [XnbProperty]
        public bool Dusk { get; set; } = true;

        [XnbProperty]
        public bool Dawn { get; set; } = true;

        [XnbProperty]
        public bool FractionalTime { get; set; }

        [XnbProperty]
        public bool OneAtATime { get; set; }

        [XnbProperty]
        public bool CutOffTail { get; set; }
    }
}
