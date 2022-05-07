using System.Numerics;

namespace FEZEngine.Structure
{
    class Volume
    {
        public FaceOrientation[]? Orientations;
        public Vector3 From;
        public Vector3 To;
        public VolumeActorSettings? ActorSettings;
    }
}
