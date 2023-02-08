using FEZRepacker.Converter.Definitions.FezEngine;
using FEZRepacker.Converter.Definitions.FezEngine.Structure;
using FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting;
using System.Numerics;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomStructures
{
    // basically the original Level class, but more arranged for JSON exporting
    // and slightly changed to get rid of redundancy
    internal class ModifiedLevel
    {
        // main parameters
        public string Name { get; set; }
        public LevelNodeType NodeType { get; set; }
        public Vector3 Size { get; set; }
        public TrileFace StartingPosition { get; set; }

        public bool Flat { get; set; }
        public bool Quantum { get; set; }
        public bool Descending { get; set; }
        public bool Loops { get; set; }
        public bool Rainy { get; set; }

        // visual aspects
        public float BaseDiffuse { get; set; }
        public float BaseAmbient { get; set; }
        public string SkyName { get; set; }
        public bool SkipPostProcess { get; set; }
        public string GomezHaloName { get; set; }
        public bool HaloFiltering { get; set; }
        public bool BlinkingAlpha { get; set; }
        public float WaterHeight { get; set; }
        public LiquidType WaterType { get; set; }

        // music and sound related
        public string SongName { get; set; }
        public List<string> MutedLoops { get; set; }
        public List<AmbienceTrack> AmbienceTracks { get; set; }
        public string SequenceSamplesPath { get; set; }
        public bool LowPass { get; set; }

        // these are presumably unused
        //public int FarAwayPlaceFadeOutStart { get; set; }
        //public int FarAwayPlaceFadeOutLength { get; set; }

        public string TrileSetName { get; set; }
        public List<ModifiedTrile> Triles { get; set; }
        public Dictionary<int, ModifiedTrileGroup> Groups { get; set; }
        public Dictionary<int, ModifiedVolume> Volumes { get; set; }
        public Dictionary<int, Script> Scripts { get; set; }
        public Dictionary<int, ModifiedArtObjectInstance> ArtObjects { get; set; }
        public Dictionary<int, BackgroundPlane> BackgroundPlanes { get; set; }
        public Dictionary<int, MovementPath> Paths { get; set; }
        public Dictionary<int, NpcInstance> NonPlayerCharacters { get; set; }

        public ModifiedLevel()
        {
            Name = "";
            StartingPosition = new();
            SkyName = "";
            GomezHaloName = "";
            SongName = "";
            MutedLoops = new();
            AmbienceTracks = new();
            SequenceSamplesPath = "";
            TrileSetName = "";
            Triles = new();
            Groups = new();
            Volumes = new();
            Scripts = new();
            ArtObjects = new();
            BackgroundPlanes = new();
            Paths = new();
            NonPlayerCharacters = new();
        }

        public ModifiedLevel(Level level)
        {
            // copy over unchanged parameters
            Name = level.Name;
            NodeType = level.NodeType;
            Size = level.Size;
            StartingPosition = level.StartingFace;
            Flat = level.Flat;
            Quantum = level.Quantum;
            Descending = level.Descending;
            Loops = level.Loops;
            Rainy = level.Rainy;
            BaseDiffuse = level.BaseDiffuse;
            BaseAmbient = level.BaseAmbient;
            SkyName = level.SkyName;
            SkipPostProcess = level.SkipPostProcess;
            GomezHaloName = level.GomezHaloName;
            HaloFiltering = level.HaloFiltering;
            BlinkingAlpha = level.BlinkingAlpha;
            WaterHeight = level.WaterHeight;
            WaterType = level.WaterType;
            SongName = level.SongName;
            //FarAwayPlaceFadeOutStart = level.FarAwayPlaceFadeOutStart;
            //FarAwayPlaceFadeOutLength = level.FarAwayPlaceFadeOutLength;
            MutedLoops = level.MutedLoops;
            AmbienceTracks = level.AmbienceTracks;
            SequenceSamplesPath = level.SequenceSamplesPath;
            LowPass = level.LowPass;
            TrileSetName = level.TrileSetName;
            Scripts = level.Scripts;
            BackgroundPlanes = level.BackgroundPlanes;
            Paths = level.Paths;
            NonPlayerCharacters = level.NonPlayerCharacters;

            // sort tiles into modified structures
            Triles = new();
            foreach(var trileRecord in level.Triles)
            {
                var pos = trileRecord.Key;
                var instance = trileRecord.Value;
                Triles.Add(new ModifiedTrile(pos, instance));
                foreach(var overlapping in instance.OverlappedTriples)
                {
                    Triles.Add(new ModifiedTrile(pos, overlapping));
                }
            }

            // create groups of modified paths
            Groups = level.Groups.ToDictionary(pair=>pair.Key, pair=>new ModifiedTrileGroup(pair.Value));

            // convert volumes
            Volumes = level.Volumes.ToDictionary(pair => pair.Key, pair => new ModifiedVolume(pair.Value));

            // convert art objects
            ArtObjects = level.ArtObjects.ToDictionary(pair => pair.Key, pair => new ModifiedArtObjectInstance(pair.Value));
        }

        public Level ToOriginal()
        {
            Level level = new Level();

            // copy over unchanged parameters
            level.Name = Name;
            level.NodeType = NodeType;
            level.Size = Size;
            level.StartingFace = StartingPosition;
            level.Flat = Flat;
            level.Quantum = Quantum;
            level.Descending = Descending;
            level.Loops = Loops;
            level.Rainy = Rainy;
            level.BaseDiffuse = BaseDiffuse;
            level.BaseAmbient = BaseAmbient;
            level.SkyName = SkyName;
            level.SkipPostProcess = SkipPostProcess;
            level.GomezHaloName = GomezHaloName;
            level.HaloFiltering = HaloFiltering;
            level.BlinkingAlpha = BlinkingAlpha;
            level.WaterHeight = WaterHeight;
            level.WaterType = WaterType;
            level.SongName = SongName;
            //level.FarAwayPlaceFadeOutStart = FarAwayPlaceFadeOutStart;
            //level.FarAwayPlaceFadeOutLength = FarAwayPlaceFadeOutLength;
            level.MutedLoops = MutedLoops;
            level.AmbienceTracks = AmbienceTracks;
            level.SequenceSamplesPath = SequenceSamplesPath;
            level.LowPass = LowPass;
            level.TrileSetName = TrileSetName;
            level.Scripts = Scripts;
            level.BackgroundPlanes = BackgroundPlanes;
            level.Paths = Paths;
            level.NonPlayerCharacters = NonPlayerCharacters;

            // parse triles back into their original structure
            foreach(var modTrile in Triles)
            {
                var trile = modTrile.ToOriginal();
                if (!level.Triles.ContainsKey(modTrile.Position))
                {
                    level.Triles[modTrile.Position] = trile;
                }
                else
                {
                    level.Triles[modTrile.Position].OverlappedTriples.Add(trile);
                }
            }

            // put trile instances back into groups
            level.Groups = Groups.ToDictionary(pair => pair.Key, pair => pair.Value.ToOriginal(level));

            // convert volumes back
            level.Volumes = Volumes.ToDictionary(pair => pair.Key, pair => pair.Value.ToOriginal());

            // convert art objects back
            level.ArtObjects = ArtObjects.ToDictionary(pair => pair.Key, pair => pair.Value.ToOriginal());

            return level;
        }
    }
}
