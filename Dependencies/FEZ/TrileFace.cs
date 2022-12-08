namespace FEZEngine.Structure
{
    class TrileFace
    {
        public TrileEmplacement Id { get; set; }
        public FaceOrientation Face { get; set; }


        public TrileFace()
        {
            Id = new();
        }
    }
}
