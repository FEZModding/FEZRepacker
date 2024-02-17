using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.TrileInstance")]
    [XnbReaderType("FezEngine.Readers.TrileInstanceReader")]
    public class TrileInstance
    {
        [XnbProperty]
        public Vector3 Position { get; set; }

        [XnbProperty]
        public int TrileId { get; set; }

        [XnbProperty]
        public byte PhiLight { get; set; }

        [XnbProperty(UseConverter = true, Optional = true)]
        public TrileInstanceActorSettings? ActorSettings { get; set; } = null;

        [XnbProperty(UseConverter = true)]
        public List<TrileInstance> OverlappedTriles { get; set; } = new();
    }
}
