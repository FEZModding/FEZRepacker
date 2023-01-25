using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.TrileInstance")]
    [XnbReaderType("FezEngine.Readers.TrileInstanceReader")]
    internal class TrileInstance
    {
        [XnbProperty]
        public Vector3 Position { get; set; }

        [XnbProperty]
        public int TrileId { get; set; }

        [XnbProperty]
        public byte PhiLight { get; set; }

        [XnbProperty(UseConverter = true, Optional = true)]
        public TrileInstanceActorSettings? ActorSettings { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<TrileInstance> OverlappedTriples { get; set; }


        public TrileInstance()
        {
            ActorSettings = null;
            OverlappedTriples = new();
        }
    }
}
