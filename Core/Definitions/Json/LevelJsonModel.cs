
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.Helpers;

namespace FEZRepacker.Core.Definitions.Json
{
    public class LevelJsonModel : JsonModel<Level>
    {
        public string Name { get; set; } = "";
        public LevelNodeType NodeType { get; set; }
        public Vector3 Size { get; set; }
        public TrileFace StartingPosition { get; set; } = new();
        public bool Flat { get; set; }
        public bool Quantum { get; set; }
        public bool Descending { get; set; }
        public bool Loops { get; set; }
        public bool Rainy { get; set; }
        public float BaseDiffuse { get; set; }
        public float BaseAmbient { get; set; }
        public string SkyName { get; set; } = "";
        public bool SkipPostProcess { get; set; }
        public string GomezHaloName { get; set; } = "";
        public bool HaloFiltering { get; set; }
        public bool BlinkingAlpha { get; set; }
        public float WaterHeight { get; set; }
        public LiquidType WaterType { get; set; }
        public string SongName { get; set; } = "";
        public List<string> MutedLoops { get; set; } = new();
        public List<AmbienceTrack> AmbienceTracks { get; set; } = new();
        public string SequenceSamplesPath { get; set; } = "";
        public bool LowPass { get; set; }
        public int FAPFadeOutStart { get; set; }
        public int FAPFadeOutLength { get; set; }
        public string TrileSetName { get; set; } = "";
        public List<TrileInstanceJsonModel> Triles { get; set; } = new();
        public IDictionary<int, TrileGroupJsonModel> Groups { get; set; } = new OrderedDictionary<int, TrileGroupJsonModel>();
        public IDictionary<int, Volume> Volumes { get; set; } = new OrderedDictionary<int, Volume>();
        public IDictionary<int, Script> Scripts { get; set; } = new OrderedDictionary<int, Script>();
        public IDictionary<int, ArtObjectInstance> ArtObjects { get; set; } = new OrderedDictionary<int, ArtObjectInstance>();
        public IDictionary<int, BackgroundPlane> BackgroundPlanes { get; set; } = new OrderedDictionary<int, BackgroundPlane>();
        public IDictionary<int, MovementPath> Paths { get; set; } = new OrderedDictionary<int, MovementPath>();
        public IDictionary<int, NpcInstance> NonPlayerCharacters { get; set; } = new OrderedDictionary<int, NpcInstance>();

        public LevelJsonModel()
        {

        }

        public LevelJsonModel(Level level) : this()
        {
            SerializeFrom(level);
        }

        public Level Deserialize()
        {
            var level = new Level()
            {
                Name = Name,
                NodeType = NodeType,
                Size = Size,
                StartingFace = StartingPosition,
                Flat = Flat,
                Quantum = Quantum,
                Descending = Descending,
                Loops = Loops,
                Rainy = Rainy,
                BaseDiffuse = BaseDiffuse,
                BaseAmbient = BaseAmbient,
                SkyName = SkyName,
                SkipPostProcess = SkipPostProcess,
                GomezHaloName = GomezHaloName,
                HaloFiltering = HaloFiltering,
                BlinkingAlpha = BlinkingAlpha,
                WaterHeight = WaterHeight,
                WaterType = WaterType,
                SongName = SongName,
                FAPFadeOutStart = FAPFadeOutStart,
                FAPFadeOutLength = FAPFadeOutLength,
                MutedLoops = MutedLoops,
                AmbienceTracks = AmbienceTracks,
                SequenceSamplesPath = SequenceSamplesPath,
                LowPass = LowPass,
                TrileSetName = TrileSetName,
                Scripts = Scripts,
                BackgroundPlanes = BackgroundPlanes,
                Paths = Paths,
                NonPlayerCharacters = NonPlayerCharacters,
                Volumes = Volumes,
                ArtObjects = ArtObjects,
            };

            level.Triles = new OrderedDictionary<TrileEmplacement, TrileInstance>();
            
            foreach (var trileModel in Triles)
            {
                var trile = trileModel.Deserialize();
                
                if (level.Triles.TryGetValue(trileModel.Emplacement, out var existingTrile))
                {
                    if (existingTrile.OverlappedTriles == null)
                    {
                        existingTrile.OverlappedTriles = new List<TrileInstance>();
                    }
                    
                    level.Triles[trileModel.Emplacement].OverlappedTriles.Add(trile);
                    continue;
                }
                
                level.Triles[trileModel.Emplacement] = trile;
            }

            level.Groups = new Dictionary<int, TrileGroup>();
            foreach (var groupModel in Groups)
            {
                level.Groups[groupModel.Key] = groupModel.Value.Deserialize();
            }

            return level;
        }

        public void SerializeFrom(Level level)
        {
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
            FAPFadeOutStart = level.FAPFadeOutStart;
            FAPFadeOutLength = level.FAPFadeOutLength;
            MutedLoops = level.MutedLoops;
            AmbienceTracks = level.AmbienceTracks;
            SequenceSamplesPath = level.SequenceSamplesPath;
            LowPass = level.LowPass;
            TrileSetName = level.TrileSetName;
            Scripts = level.Scripts;
            BackgroundPlanes = level.BackgroundPlanes;
            Paths = level.Paths;
            NonPlayerCharacters = level.NonPlayerCharacters;
            Volumes = level.Volumes;
            ArtObjects = level.ArtObjects;

            // sort tiles into modified structures
            Triles = new();
            foreach (var trileRecord in level.Triles)
            {
                var pos = trileRecord.Key;
                var instance = trileRecord.Value;
                Triles.Add(new TrileInstanceJsonModel(pos, instance));

                if (instance.OverlappedTriles == null)
                {
                    continue;
                }
                
                foreach (var overlapping in instance.OverlappedTriles)
                {
                    Triles.Add(new TrileInstanceJsonModel(pos, overlapping));
                }
            }

            // create groups of modified paths
            Groups = new Dictionary<int, TrileGroupJsonModel>();
            foreach (var pair in level.Groups)
            {
                Groups[pair.Key] = new TrileGroupJsonModel(pair.Value);
            }
        }
    }
}
