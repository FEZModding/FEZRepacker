using System.Numerics;

namespace FEZEngine.Structure
{
    class Level
    {
        public string? Name;
        public Vector3 Size;
        public TrileFace? StartingFace;
        public string? SequenceSamplesPath;
        public bool Flat;
        public bool SkipPostProcess;
        public float BaseDiffuse;
        public float BaseAmbient;
        public string? GomezHaloName;
        public bool HaloFiltering;
        public bool BlinkingAlpha;
        public bool Loops;
    }
}
