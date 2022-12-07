using System.Numerics;

namespace FEZEngine.Structure
{
    class ArtObjectInstance
    {
        public string Name = "";
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public ArtObjectActorSettings? ActorSettings;
    }
}
