namespace FEZRepacker.Core.Definitions.Game.TrackedSong
{
    [XnbType("FezEngine.Structure.TrackedSong, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.TrackedSongReader, FezEngine")]
    public class TrackedSong
    {
        [XnbProperty(UseConverter = true)]
        public List<Loop> Loops { get; set; } = new();

        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty]
        public int Tempo { get; set; }

        [XnbProperty]
        public int TimeSignature { get; set; }

        [XnbProperty(UseConverter = true)]
        public ShardNotes[] Notes { get; set; } = { };

        [XnbProperty(UseConverter = true)]
        public AssembleChords AssembleChord { get; set; }

        [XnbProperty]
        public bool RandomOrdering { get; set; }

        [XnbProperty(UseConverter = true)]
        public int[]? CustomOrdering { get; set; }
    }
}
