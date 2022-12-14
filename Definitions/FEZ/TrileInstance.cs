using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FEZEngine.Structure
{
    [XNBType("FezEngine.Readers.TrileInstanceReader")]
    public class TrileInstance
    {
        [XNBProperty]
        public Vector3 Position { get; set; }

        [XNBProperty]
        public int TrileId { get; set; }

        [XNBProperty]
        public byte PhiLight { get; set; }

        [XNBProperty(UseConverter = true, Optional = true)]
        public TrileInstanceActorSettings? ActorSettings { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<TrileInstance> OverlappedTriples { get; set; }


        public TrileInstance()
        {
            ActorSettings = null;
            OverlappedTriples = new();
        }
    }
}
