
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Json
{
    public class LevelJsonModel : JsonModel<Level>
    {
        public string Name { get; set; }
        public LevelNodeType NodeType { get; set; }
        public Vector3 Size { get; set; }
        public TrileFace StartingPosition { get; set; }
        public bool Flat { get; set; }
        public bool Quantum { get; set; }
        public bool Descending { get; set; }
        public bool Loops { get; set; }
        public bool Rainy { get; set; }
        public float BaseDiffuse { get; set; }
        public float BaseAmbient { get; set; }
        public string SkyName { get; set; }
        public bool SkipPostProcess { get; set; }
        public string GomezHaloName { get; set; }
        public bool HaloFiltering { get; set; }
        public bool BlinkingAlpha { get; set; }
        public float WaterHeight { get; set; }
        public LiquidType WaterType { get; set; }
        public string SongName { get; set; }
        public List<string> MutedLoops { get; set; }
        public List<AmbienceTrack> AmbienceTracks { get; set; }
        public string SequenceSamplesPath { get; set; }
        public bool LowPass { get; set; }
        public int FAPFadeOutStart { get; set; }
        public int FAPFadeOutLength { get; set; }
        public string TrileSetName { get; set; }
        public List<TrileInstanceJsonModel> Triles { get; set; }
        public Dictionary<int, TrileGroupJsonModel> Groups { get; set; }
        public Dictionary<int, Volume> Volumes { get; set; }
        public Dictionary<int, Script> Scripts { get; set; }
        public Dictionary<int, ArtObjectInstance> ArtObjects { get; set; }
        public Dictionary<int, BackgroundPlane> BackgroundPlanes { get; set; }
        public Dictionary<int, MovementPath> Paths { get; set; }
        public Dictionary<int, NpcInstance> NonPlayerCharacters { get; set; }

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

            foreach (var trileModel in Triles)
            {
                var trile = trileModel.Deserialize();
                if (!level.Triles.ContainsKey(trileModel.Emplacement))
                {
                    level.Triles[trileModel.Emplacement] = trile;
                }
                else
                {
                    level.Triles[trileModel.Emplacement].OverlappedTriples.Add(trile);
                }
            }

            level.Groups = Groups.ToDictionary(pair => pair.Key, pair => pair.Value.Deserialize());

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
                foreach (var overlapping in instance.OverlappedTriples)
                {
                    Triles.Add(new TrileInstanceJsonModel(pos, overlapping));
                }
            }

            // create groups of modified paths
            Groups = level.Groups.ToDictionary(pair => pair.Key, pair => new TrileGroupJsonModel(pair.Value));
        }
    }
}
