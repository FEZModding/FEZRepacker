namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.MovementPath")]
    [XnbReaderType("FezEngine.Readers.MovementPathReader")]
    public class MovementPath
    {
        [XnbProperty(UseConverter = true)]
        public List<PathSegment> Segments { get; set; } = new();

        [XnbProperty]
        public bool NeedsTrigger { get; set; }

        [XnbProperty(UseConverter = true)]
        public PathEndBehavior EndBehavior { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SoundName { get; set; } = "";

        [XnbProperty]
        public bool IsSpline { get; set; }

        [XnbProperty]
        public float OffsetSeconds { get; set; }

        [XnbProperty]
        public bool SaveTrigger { get; set; }
    }
}