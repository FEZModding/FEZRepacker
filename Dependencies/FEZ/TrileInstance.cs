using System.Numerics;

namespace FEZEngine.Structure
{
    public class TrileInstance
    {
        public Vector3 Position { get; set; }
        public int TrileId { get; set; }
        public byte PhiLight { get; set; }
        public TrileInstanceActorSettings? ActorSettings { get; set; }
        public List<TrileInstance> OverlappedTriples { get; set; }


        public TrileInstance()
        {
            ActorSettings = null;
            OverlappedTriples = new();
        }
    }
}
