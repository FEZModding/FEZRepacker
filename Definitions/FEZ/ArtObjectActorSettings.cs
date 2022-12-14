using FEZEngine.Structure.Input;
using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FEZEngine.Structure
{
    [XNBType("FezEngine.Readers.ArtObjectActorSettingsReader")]
    class ArtObjectActorSettings
    {
        [XNBProperty]
        public bool Inactive { get; set; }

        [XNBProperty(UseConverter = true)]
        public ActorType ContainedTrile { get; set; }

        [XNBProperty(Optional = true)]
        public int? AttachedGroup { get; set; }

        [XNBProperty(UseConverter = true)]
        public Viewpoint SpinView { get; set; }

        [XNBProperty]
        public float SpinEvery { get; set; }

        [XNBProperty]
        public float SpinOffset { get; set; }

        [XNBProperty]
        public bool OffCenter { get; set; }

        [XNBProperty]
        public Vector3 RotationCenter { get; set; }

        [XNBProperty(UseConverter = true)]
        public VibrationMotor[] VibrationPattern { get; set; }

        [XNBProperty(UseConverter = true)]
        public CodeInput[] CodePattern { get; set; }

        [XNBProperty(UseConverter = true)]
        public PathSegment Segment { get; set; }

        [XNBProperty(Optional = true)]
        public int? NextNode { get; set; }

        [XNBProperty(UseConverter = true)]
        public string DestinationLevel { get; set; }

        [XNBProperty(UseConverter = true)]
        public string TreasureMapName { get; set; }

        [XNBProperty(UseConverter = true)]
        public FaceOrientation[] InvisibleSides { get; set; }

        [XNBProperty]
        public float TimeswitchWindBackSpeed { get; set; }


        public ArtObjectActorSettings()
        {
            VibrationPattern = new VibrationMotor[0];
            CodePattern = new CodeInput[0];
            Segment = new();
            DestinationLevel = "";
            TreasureMapName = "";
            InvisibleSides = new FaceOrientation[0];
        }
    }
}
