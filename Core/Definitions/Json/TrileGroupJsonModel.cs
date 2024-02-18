
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Json
{
    // storing TrileEmplacements instead of TrileInstances since the game is using 
    // just that to identify TrileInstance from Level's Triles dictionary anyway.
    public class TrileGroupJsonModel : JsonModel<TrileGroup>
    {
        public List<TrileEmplacement> Triles { get; set; }
        public MovementPath Path { get; set; }
        public bool Heavy { get; set; }
        public ActorType ActorType { get; set; }
        public float GeyserOffset { get; set; }
        public float GeyserPauseFor { get; set; }
        public float GeyserLiftFor { get; set; }
        public float GeyserApexHeight { get; set; }
        public Vector3 SpinCenter { get; set; }
        public bool SpinClockwise { get; set; }
        public float SpinFrequency { get; set; }
        public bool SpinNeedsTriggering { get; set; }
        public bool Spin180Degrees { get; set; }
        public bool FallOnRotate { get; set; }
        public float SpinOffset { get; set; }
        public string AssociatedSound { get; set; }

        public TrileGroupJsonModel()
        {
            Triles = new();
            Path = new();
            AssociatedSound = "";
        }
        public TrileGroupJsonModel(TrileGroup data) : this()
        {
            SerializeFrom(data);
        }

        public TrileGroup Deserialize()
        {
            var trileGroup = new TrileGroup()
            {
                Path = Path,
                Heavy = Heavy,
                ActorType = ActorType,
                GeyserOffset = GeyserOffset,
                GeyserPauseFor = GeyserPauseFor,
                GeyserLiftFor = GeyserLiftFor,
                GeyserApexHeight = GeyserApexHeight,
                SpinCenter = SpinCenter,
                SpinClockwise = SpinClockwise,
                SpinFrequency = SpinFrequency,
                SpinNeedsTriggering = SpinNeedsTriggering,
                Spin180Degrees = Spin180Degrees,
                FallOnRotate = FallOnRotate,
                SpinOffset = SpinOffset,
                AssociatedSound = AssociatedSound,
            };

            trileGroup.Triles = Triles
                .Select(x => new TrileInstance() { Position = new(x.X, x.Y, x.Z) })
                .ToList();

            return trileGroup;
        }

        private void CopyBasicPropertiesOverFrom(TrileGroup trileGroup)
        {
            Path = trileGroup.Path!;
            Heavy = trileGroup.Heavy;
            ActorType = trileGroup.ActorType;
            GeyserOffset = trileGroup.GeyserOffset;
            GeyserPauseFor = trileGroup.GeyserPauseFor;
            GeyserLiftFor = trileGroup.GeyserLiftFor;
            GeyserApexHeight = trileGroup.GeyserApexHeight;
            SpinCenter = trileGroup.SpinCenter;
            SpinClockwise = trileGroup.SpinClockwise;
            SpinFrequency = trileGroup.SpinFrequency;
            SpinNeedsTriggering = trileGroup.SpinNeedsTriggering;
            Spin180Degrees = trileGroup.Spin180Degrees;
            FallOnRotate = trileGroup.FallOnRotate;
            SpinOffset = trileGroup.SpinOffset;
            AssociatedSound = trileGroup.AssociatedSound;
        }

        public void SerializeFrom(TrileGroup trileGroup)
        {
            CopyBasicPropertiesOverFrom(trileGroup);

            Triles = trileGroup.Triles.Select(x => new TrileEmplacement(x.Position)).ToList();
        }
    }
}
