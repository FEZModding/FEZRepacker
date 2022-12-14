using FEZRepacker.XNB.Attributes;

namespace FEZEngine.Structure
{
    [XNBType("FezEngine.Readers.TrileEmplacementReader")]
    public class TrileEmplacement
    {
        [XNBProperty]
        public int X { get; set; }

        [XNBProperty]
        public int Y { get; set; }

        [XNBProperty]
        public int Z { get; set; }

        public TrileEmplacement() {}

        public TrileEmplacement(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
