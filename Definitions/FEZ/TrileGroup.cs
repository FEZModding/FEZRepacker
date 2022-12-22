using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FezEngine.Structure
{
    [XNBType("FezEngine.Readers.TrileGroupReader")]
    public class TrileGroup
	{
        [XNBProperty(UseConverter = true)]
		public List<TrileInstance> Triles { get; set; }

        [XNBProperty(UseConverter = true, Optional = true, SkipIdentifier = true)] 
        public MovementPath? Path { get; set; }
        
        [XNBProperty]
        public bool Heavy { get; set; }
        
        [XNBProperty(UseConverter = true)]
        public ActorType ActorType { get; set; }
        
        [XNBProperty]
        public float GeyserOffset { get; set; }
        
        [XNBProperty]
        public float GeyserPauseFor { get; set; }
        
        [XNBProperty]
        public float GeyserLiftFor { get; set; }
        
        [XNBProperty]
        public float GeyserApexHeight { get; set; }
        
        [XNBProperty]
        public Vector3 SpinCenter { get; set; }
        
        [XNBProperty]
        public bool SpinClockwise { get; set; }
        
        [XNBProperty]
        public float SpinFrequency { get; set; }
        
        [XNBProperty]
        public bool SpinNeedsTriggering { get; set; }
        
        [XNBProperty]
        public bool Spin180Degrees { get; set; }
        
        [XNBProperty]
        public bool FallOnRotate { get; set; }
        
        [XNBProperty]
        public float SpinOffset { get; set; }
        
        [XNBProperty(UseConverter = true)]
        public string AssociatedSound { get; set; }


        public TrileGroup()
        {
            Triles = new List<TrileInstance>();
            Path = null;
            AssociatedSound = "";
        }
    }
}