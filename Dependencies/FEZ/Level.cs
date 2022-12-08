using FEZEngine.Structure.Scripting;
using System.Numerics;

namespace FEZEngine.Structure
{
    class Level
    {
        // General level informations
        public string Name { get; set; }
        public Vector3 Size { get; set; }
        public TrileFace StartingFace { get; set; }
        public LevelNodeType NodeType { get; set; }

        // Gameplay level parameters and flags
        public bool Flat { get; set; }
        public bool Quantum { get; set; }
        public bool Descending { get; set; }
        public bool Loops { get; set; }
        public string SkyName { get; set; }
        public LiquidType WaterType { get; set; }
        public float WaterHeight { get; set; }

        // Visual level parameters and flags
        public bool Rainy { get; set; }
        public string GomezHaloName { get; set; }
        public bool HaloFiltering { get; set; }
        public bool BlinkingAlpha { get; set; }
        public bool SkipPostProcess { get; set; }
        public float BaseDiffuse { get; set; }
        public float BaseAmbient { get; set; }

        // Music / sequence related
        public string SongName { get; set; }
        public List<string> MutedLoops { get; set; }
        public List<AmbienceTrack> AmbienceTracks { get; set; }
        public bool LowPass { get; set; }
        public int FarAwayPlaceFadeOutStart { get; set; }
        public int FarAwayPlaceFadeOutLength { get; set; }
        public string SequenceSamplesPath { get; set; }

        // Level logic
        public Dictionary<int, Volume> Volumes { get; set; }
        public Dictionary<int, Script> Scripts { get; set; }

        // Level structure
        public string TrileSetName { get; set; }
        public Dictionary<TrileEmplacement, TrileInstance> Triles { get; set; }
        public Dictionary<int, ArtObjectInstance> ArtObjects { get; set; }
        public Dictionary<int, BackgroundPlane> BackgroundPlanes { get; set; }
        public Dictionary<int, NpcInstance> NonPlayerCharacters { get; set; }
        public Dictionary<int, TrileGroup> Groups { get; set; }
        public Dictionary<int, MovementPath> Paths { get; set; }


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
