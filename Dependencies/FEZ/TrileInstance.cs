using System.Numerics;

namespace FEZEngine.Structure
{
    public class TrileInstance
    {
        public Vector3 Position;
        public int TrileId;
        public byte PhiLight;
        public InstanceActorSettings? ActorSettings;
        public List<TrileInstance>? OverlappedTriples;
    }
}
