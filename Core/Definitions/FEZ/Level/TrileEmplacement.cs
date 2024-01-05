using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.TrileEmplacement")]
    [XnbReaderType("FezEngine.Readers.TrileEmplacementReader")]
    internal class TrileEmplacement : IEquatable<TrileEmplacement>, IComparable<TrileEmplacement>
    {
        [XnbProperty]
        public int X { get; set; }

        [XnbProperty]
        public int Y { get; set; }

        [XnbProperty]
        public int Z { get; set; }

        public TrileEmplacement() { }

        public TrileEmplacement(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public TrileEmplacement(Vector3 vec)
        {
            X = (int)Math.Round(vec.X);
            Y = (int)Math.Round(vec.Y);
            Z = (int)Math.Round(vec.Z);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                TrileEmplacement p = (TrileEmplacement)obj;
                return (X == p.X) && (Y == p.Y) && (Z == p.Z);
            }
        }

        public override int GetHashCode()
        {
            return X ^ (Y << 10) ^ (Z << 20);
        }

        public bool Equals(TrileEmplacement? other)
        {
            if (other == null) return false;
            return (X == other.X) && (Y == other.Y) && (Z == other.Z);
        }

        public int CompareTo(TrileEmplacement? other)
        {
            if (other == null) return -1;
            int num = X.CompareTo(other.X);
            if (num == 0)
            {
                num = Y.CompareTo(other.Y);
                if (num == 0)
                {
                    num = Z.CompareTo(other.Z);
                }
            }
            return num;
        }
    }
}
