using FEZEngine.Structure.Scripting;
using System.Numerics;

namespace FEZEngine.Structure
{
    class Level
    {
        public string Name = "";
        public Vector3 Size;
        public TrileFace StartingFace = new TrileFace();
        public string SequenceSamplesPath = "";
        public bool Flat;
        public bool SkipPostProcess;
        public float BaseDiffuse;
        public float BaseAmbient;
        public string GomezHaloName = "";
        public bool HaloFiltering;
        public bool BlinkingAlpha;
        public bool Loops;
        public LiquidType WaterType;
        public float WaterHeight;
        public string SkyName = "";
        public string TrileSetName = "";
        public Dictionary<int, Volume> Volumes = new Dictionary<int, Volume>();
        public Dictionary<int, Script> Scripts = new Dictionary<int, Script>();
        public string SongName = "";
        public int FarAwayPlaceFadeOutStart;
        public int FarAwayPlaceFadeOutLength;

        public Dictionary<TrileEmplacement, TrileInstance> Triles = new();
        public Dictionary<int, ArtObjectInstance> ArtObjects = new();
        public Dictionary<int, BackgroundPlane> BackgroundPlanes = new();
        public Dictionary<int, TrileGroup> Groups = new();
        public Dictionary<int, NpcInstance> NonPlayerCharacters = new();
        public Dictionary<int, MovementPath> Paths = new();

        public bool Descending;
        public bool Rainy;
        public bool LowPass;
        public List<string> MutedLoops = new();
        public List<AmbienceTrack> AmbienceTracks = new();
        public LevelNodeType NodeType;
        public bool Quantum;
    }
}
