namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.AmbienceTrack, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.AmbienceTrackReader, FezEngine")]
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
