namespace FEZRepacker.Core.Definitions.Game.TrackedSong
{
    [XnbType("FezEngine.Structure.TrackedSong, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.TrackedSongReader, FezEngine")]
    public class TrackedSong
    {
        [XnbProperty(UseConverter = true)]
        public List<Loop>? Loops { get; set; } = new();

        [XnbProperty]
        public string? Name { get; set; } = "Untitled";

        [XnbProperty]
        public int Tempo { get; set; } = 60;

        [XnbProperty]
        public int TimeSignature { get; set; } = 4;

        [XnbProperty(UseConverter = true)]
        public ShardNotes[]? Notes { get; set; } = new[]
        {
            ShardNotes.C2,
            ShardNotes.D2,
            ShardNotes.E2,
            ShardNotes.F2,
            ShardNotes.G2,
            ShardNotes.A2,
            ShardNotes.B2,
            ShardNotes.C3
        };

        [XnbProperty(UseConverter = true)]
        public AssembleChords AssembleChord { get; set; }

        [XnbProperty]
        public bool RandomOrdering { get; set; }

        [XnbProperty(UseConverter = true)]
        public int[]? CustomOrdering { get; set; }
    }
}
