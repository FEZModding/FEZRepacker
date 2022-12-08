using System.Numerics;

namespace FEZEngine.Structure
{
	public class TrileGroup
	{
		public List<TrileInstance>? Triles;
		public MovementPath? Path;
		public bool Heavy;
		public ActorType ActorType;
        public float GeyserOffset;
        public float GeyserPauseFor;
        public float GeyserLiftFor;
        public float GeyserApexHeight;
        public Vector3 SpinCenter;
        public bool SpinClockwise;
        public float SpinFrequency;
        public bool SpinNeedsTriggering;
        public bool Spin180Degrees;
        public bool FallOnRotate;
        public float SpinOffset;
        public string AssociatedSound = "";
    }
}