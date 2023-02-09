namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.MovementPath")]
    [XnbReaderType("FezEngine.Readers.MovementPathReader")]
    internal class MovementPath
    {
        [XnbProperty(UseConverter = true)]
        public List<PathSegment> Segments { get; set; }

        [XnbProperty]
        public bool NeedsTrigger { get; set; }

        [XnbProperty(UseConverter = true)]
        public PathEndBehavior EndBehavior { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SoundName { get; set; }

        [XnbProperty]
        public bool IsSpline { get; set; }

        [XnbProperty]
        public float OffsetSeconds { get; set; }

        [XnbProperty]
        public bool SaveTrigger { get; set; }


        public MovementPath()
        {
            Segments = new();
            SoundName = "";
        }
    }
}