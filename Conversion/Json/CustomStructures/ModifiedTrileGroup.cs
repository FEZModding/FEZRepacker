using FEZRepacker.Definitions.FezEngine.Structure;
using System.Linq;
using System.Numerics;

namespace FEZRepacker.Conversion.Json.CustomStructures
{
    // storing TrileEmplacements instead of TrileInstances since the game is using 
    // just that to identify TrileInstance from Level's Triles dictionary anyway.
    class ModifiedTrileGroup
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

        public ModifiedTrileGroup()
        {
            Triles = new();
            Path = new MovementPath();
            AssociatedSound = "";
        }

        public ModifiedTrileGroup(TrileGroup trileGroup)
        {
            // copy over unchanged parameters 
            Path = trileGroup.Path;
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

            // create emplacement list
            Triles = trileGroup.Triles.Select(x => new TrileEmplacement(x.Position)).ToList();
        }

        public TrileGroup ToOriginal(Level level)
        {
            TrileGroup trileGroup = new TrileGroup();

            // copy over unchanged parameters
            trileGroup.Path = Path;
            trileGroup.Heavy = Heavy;
            trileGroup.ActorType = ActorType;
            trileGroup.GeyserOffset = GeyserOffset;
            trileGroup.GeyserPauseFor = GeyserPauseFor;
            trileGroup.GeyserLiftFor = GeyserLiftFor;
            trileGroup.GeyserApexHeight = GeyserApexHeight;
            trileGroup.SpinCenter = SpinCenter;
            trileGroup.SpinClockwise = SpinClockwise;
            trileGroup.SpinFrequency = SpinFrequency;
            trileGroup.SpinNeedsTriggering = SpinNeedsTriggering;
            trileGroup.Spin180Degrees = Spin180Degrees;
            trileGroup.FallOnRotate = FallOnRotate;
            trileGroup.SpinOffset = SpinOffset;
            trileGroup.AssociatedSound = AssociatedSound;

            // create triles list out of emplacements
            trileGroup.Triles = Triles.Select(x => level.Triles[x]).Where(x => x != null).ToList();

            return trileGroup;
        }
    }
}
