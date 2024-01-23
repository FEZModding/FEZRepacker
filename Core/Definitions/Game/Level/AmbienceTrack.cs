namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.AmbienceTrack")]
    [XnbReaderType("FezEngine.Readers.AmbienceTrackReader")]
    public class AmbienceTrack
    {
        [XnbProperty(UseConverter = true)]
        public string Name { get; set; } = "";

        [XnbProperty]
        public bool Day { get; set; }

        [XnbProperty]
        public bool Dusk { get; set; }

        [XnbProperty]
        public bool Night { get; set; }

        [XnbProperty]
        public bool Dawn { get; set; }
    }
}
