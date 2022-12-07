using FEZEngine.Structure.Input;
using System.Numerics;

namespace FEZEngine.Structure
{
    class ArtObjectActorSettings
    {
        public bool Inactive;
        public ActorType ContainedTrile;
        public int? AttachedGroup;
        public Viewpoint SpinView;
        public float SpinEvery;
        public float SpinOffset;
        public bool OffCenter;
        public Vector3 RotationCenter;
        public VibrationMotor[]? VibrationPattern;
        public CodeInput[]? CodePattern;
        public PathSegment? Segment;
        public int? NextNode;
        public string DestinationLevel = "";
        public string TreasureMapName = "";
        public FaceOrientation[]? InvisibleSides;
        public float TimeswitchWindBackSpeed;
    }
}
