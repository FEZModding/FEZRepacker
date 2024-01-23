using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.Level")]
    [XnbReaderType("FezEngine.Readers.LevelReader")]
    public class Level
    {
        [XnbProperty(UseConverter = true)]
        public string Name { get; set; }

        [XnbProperty]
        public Vector3 Size { get; set; }

        [XnbProperty(UseConverter = true)]
        public TrileFace StartingFace { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SequenceSamplesPath { get; set; }

        [XnbProperty]
        public bool Flat { get; set; }

        [XnbProperty]
        public bool SkipPostProcess { get; set; }

        [XnbProperty]
        public float BaseDiffuse { get; set; }

        [XnbProperty]
        public float BaseAmbient { get; set; }

        [XnbProperty(UseConverter = true)]
        public string GomezHaloName { get; set; }

        [XnbProperty]
        public bool HaloFiltering { get; set; }

        [XnbProperty]
        public bool BlinkingAlpha { get; set; }

        [XnbProperty]
        public bool Loops { get; set; }

        [XnbProperty(UseConverter = true)]
        public LiquidType WaterType { get; set; }

        [XnbProperty]
        public float WaterHeight { get; set; }

        [XnbProperty]
        public string SkyName { get; set; }

        [XnbProperty(UseConverter = true)]
        public string TrileSetName { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, Volume> Volumes { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, Script> Scripts { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SongName { get; set; }

        [XnbProperty]
        public int FAPFadeOutStart { get; set; }

        [XnbProperty]
        public int FAPFadeOutLength { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<TrileEmplacement, TrileInstance> Triles { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, ArtObjectInstance> ArtObjects { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, BackgroundPlane> BackgroundPlanes { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, TrileGroup> Groups { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, NpcInstance> NonPlayerCharacters { get; set; }

        [XnbProperty(UseConverter = true)]
        public Dictionary<int, MovementPath> Paths { get; set; }

        [XnbProperty]
        public bool Descending { get; set; }

        [XnbProperty]
        public bool Rainy { get; set; }

        [XnbProperty]
        public bool LowPass { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<string> MutedLoops { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<AmbienceTrack> AmbienceTracks { get; set; }

        [XnbProperty(UseConverter = true)]
        public LevelNodeType NodeType { get; set; }

        [XnbProperty]
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
