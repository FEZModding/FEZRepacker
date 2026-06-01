using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.Helpers;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.Level, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.LevelReader, FezEngine")]
    public class Level
    {
        [XnbProperty(UseConverter = true)]
        public string Name { get; set; } = "";

        [XnbProperty]
        public Vector3 Size { get; set; }

        [XnbProperty(UseConverter = true)]
        public TrileFace? StartingFace { get; set; }

        [XnbProperty(UseConverter = true)]
        public string? SequenceSamplesPath { get; set; }

        [XnbProperty]
        public bool Flat { get; set; }

        [XnbProperty]
        public bool SkipPostProcess { get; set; }

        [XnbProperty]
        public float BaseDiffuse { get; set; } = 1.0f;

        [XnbProperty]
        public float BaseAmbient { get; set; } = 0.35f;

        [XnbProperty(UseConverter = true)]
        public string? GomezHaloName { get; set; }

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
        public string SkyName { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string TrileSetName { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, Volume> Volumes { get; set; } = new OrderedDictionary<int, Volume>();

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, Script> Scripts { get; set; } = new OrderedDictionary<int, Script>();

        [XnbProperty(UseConverter = true)]
        public string? SongName { get; set; }

        [XnbProperty]
        public int FAPFadeOutStart { get; set; }

        [XnbProperty]
        public int FAPFadeOutLength { get; set; }

        [XnbProperty(UseConverter = true)]
        public IDictionary<TrileEmplacement, TrileInstance> Triles { get; set; } = new OrderedDictionary<TrileEmplacement, TrileInstance>();

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, ArtObjectInstance> ArtObjects { get; set; } = new OrderedDictionary<int, ArtObjectInstance>();

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, BackgroundPlane> BackgroundPlanes { get; set; } = new OrderedDictionary<int, BackgroundPlane>();

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, TrileGroup> Groups { get; set; } = new OrderedDictionary<int, TrileGroup>();

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, NpcInstance> NonPlayerCharacters { get; set; } = new OrderedDictionary<int, NpcInstance>();

        [XnbProperty(UseConverter = true)]
        public IDictionary<int, MovementPath> Paths { get; set; } = new OrderedDictionary<int, MovementPath>();

        [XnbProperty]
        public bool Descending { get; set; }

        [XnbProperty]
        public bool Rainy { get; set; }

        [XnbProperty]
        public bool LowPass { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<string> MutedLoops { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<AmbienceTrack> AmbienceTracks { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public LevelNodeType NodeType { get; set; }

        [XnbProperty]
        public bool Quantum { get; set; }
    }
}
