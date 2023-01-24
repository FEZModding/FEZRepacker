namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Readers.TrileFaceReader")]
    internal class TrileFace
    {
        [XnbProperty(UseConverter = true)]
        public TrileEmplacement Id { get; set; }

        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }


        public TrileFace()
        {
            Id = new();
        }
    }
}
