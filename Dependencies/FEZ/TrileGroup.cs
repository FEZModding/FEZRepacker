using System.Numerics;

namespace FEZEngine.Structure
{
	public class TrileGroup
	{
		public List<TrileInstance> Triles { get; set; }
		public MovementPath Path { get; set; }
        public bool Heavy { get; set; }
        public ActorType ActorType { get; set; }
        public float GeyserOffset { get; set; }
        public float GeyserPauseFor { get; set; }
        public float GeyserLiftFor { get; set; }
        public float GeyserApexHeight { get; set; }
        public Vector3 SpinCenter { get; set; }
        public bool SpinClockwise { get; set; }
        public float SpinFrequency { get; set; }
        public bool SpinNeedsTriggering { get; set; }
        public bool Spin180Degrees { get; set; }
        public bool FallOnRotate { get; set; }
        public float SpinOffset { get; set; }
        public string AssociatedSound { get; set; }


        public TrileGroup()
        {
            Triles = new List<TrileInstance>();
            Path = new();
            AssociatedSound = "";
        }
    }
}