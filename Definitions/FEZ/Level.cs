using FezEngine.Structure.Scripting;
using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FezEngine.Structure
{
    [XNBType("FezEngine.Readers.LevelReader")]
    class Level
    {
        [XNBProperty(UseConverter = true)]
        public string Name { get; set; }

        [XNBProperty]
        public Vector3 Size { get; set; }

        [XNBProperty(UseConverter = true)]
        public TrileFace StartingFace { get; set; }

        [XNBProperty(UseConverter = true)]
        public string SequenceSamplesPath { get; set; }

        [XNBProperty]
        public bool Flat { get; set; }

        [XNBProperty]
        public bool SkipPostProcess { get; set; }

        [XNBProperty]
        public float BaseDiffuse { get; set; }

        [XNBProperty]
        public float BaseAmbient { get; set; }

        [XNBProperty(UseConverter = true)]
        public string GomezHaloName { get; set; }

        [XNBProperty]
        public bool HaloFiltering { get; set; }

        [XNBProperty]
        public bool BlinkingAlpha { get; set; }

        [XNBProperty]
        public bool Loops { get; set; }

        [XNBProperty(UseConverter = true)]
        public LiquidType WaterType { get; set; }

        [XNBProperty]
        public float WaterHeight { get; set; }

        [XNBProperty]
        public string SkyName { get; set; }

        [XNBProperty(UseConverter = true)]
        public string TrileSetName { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, Volume> Volumes { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, Script> Scripts { get; set; }

        [XNBProperty(UseConverter = true)]
        public string SongName { get; set; }

        [XNBProperty]
        public int FarAwayPlaceFadeOutStart { get; set; }

        [XNBProperty]
        public int FarAwayPlaceFadeOutLength { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<TrileEmplacement, TrileInstance> Triles { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, ArtObjectInstance> ArtObjects { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, BackgroundPlane> BackgroundPlanes { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, TrileGroup> Groups { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, NpcInstance> NonPlayerCharacters { get; set; }

        [XNBProperty(UseConverter = true)]
        public Dictionary<int, MovementPath> Paths { get; set; }

        [XNBProperty]
        public bool Descending { get; set; }

        [XNBProperty]
        public bool Rainy { get; set; }

        [XNBProperty]
        public bool LowPass { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<string> MutedLoops { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<AmbienceTrack> AmbienceTracks { get; set; }

        [XNBProperty(UseConverter = true)]
        public LevelNodeType NodeType { get; set; }

        [XNBProperty]
        public bool Quantum { get; set; }


        public Level()
        {
            Triles = new();
            Volumes = new();
            ArtObjects = new();
            BackgroundPlanes = new();
            Groups = new();
            Scripts = new();
            NonPlayerCharacters = new();
            Paths = new();
            MutedLoops = new();
            AmbienceTracks = new();
            BaseDiffuse = 1.0f;
            BaseAmbient = 0.35f;
            HaloFiltering = true;

            Name = "";
            StartingFace = new();
            SkyName = "";
            GomezHaloName = "";
            SongName = "";
            SequenceSamplesPath = "";
            TrileSetName = "";
        }
    }
}
