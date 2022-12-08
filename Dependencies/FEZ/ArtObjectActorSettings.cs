using FEZEngine.Structure.Input;
using System.Numerics;

namespace FEZEngine.Structure
{
    class ArtObjectActorSettings
    {
        public bool Inactive { get; set; }
        public ActorType ContainedTrile { get; set; }
        public int? AttachedGroup { get; set; }
        public Viewpoint SpinView { get; set; }
        public float SpinEvery { get; set; }
        public float SpinOffset { get; set; }
        public bool OffCenter { get; set; }
        public Vector3 RotationCenter { get; set; }
        public VibrationMotor[] VibrationPattern { get; set; }
        public CodeInput[] CodePattern { get; set; }
        public PathSegment Segment { get; set; }
        public int? NextNode { get; set; }
        public string DestinationLevel { get; set; }
        public string TreasureMapName { get; set; }
        public FaceOrientation[] InvisibleSides { get; set; }
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
