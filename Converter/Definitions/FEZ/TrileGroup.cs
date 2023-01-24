using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Readers.TrileGroupReader")]
    internal class TrileGroup
	{
        [XnbProperty(UseConverter = true)]
		public List<TrileInstance> Triles { get; set; }

        [XnbProperty(UseConverter = true, Optional = true, SkipIdentifier = true)] 
        public MovementPath? Path { get; set; }
        
        [XnbProperty]
        public bool Heavy { get; set; }
        
        [XnbProperty(UseConverter = true)]
        public ActorType ActorType { get; set; }
        
        [XnbProperty]
        public float GeyserOffset { get; set; }
        
        [XnbProperty]
        public float GeyserPauseFor { get; set; }
        
        [XnbProperty]
        public float GeyserLiftFor { get; set; }
        
        [XnbProperty]
        public float GeyserApexHeight { get; set; }
        
        [XnbProperty]
        public Vector3 SpinCenter { get; set; }
        
        [XnbProperty]
        public bool SpinClockwise { get; set; }
        
        [XnbProperty]
        public float SpinFrequency { get; set; }
        
        [XnbProperty]
        public bool SpinNeedsTriggering { get; set; }
        
        [XnbProperty]
        public bool Spin180Degrees { get; set; }
        
        [XnbProperty]
        public bool FallOnRotate { get; set; }
        
        [XnbProperty]
        public float SpinOffset { get; set; }
        
        [XnbProperty(UseConverter = true)]
        public string AssociatedSound { get; set; }


        public TrileGroup()
        {
            Triles = new List<TrileInstance>();
            Path = null;
            AssociatedSound = "";
        }
    }
}