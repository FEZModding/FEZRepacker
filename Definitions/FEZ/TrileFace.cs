using FEZRepacker.XNB.Attributes;

namespace FEZEngine.Structure
{
    [XNBType("FezEngine.Readers.TrileFaceReader")]
    class TrileFace
    {
        [XNBProperty(UseConverter = true)]
        public TrileEmplacement Id { get; set; }

        [XNBProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }


        public TrileFace()
        {
            Id = new();
        }
    }
}
