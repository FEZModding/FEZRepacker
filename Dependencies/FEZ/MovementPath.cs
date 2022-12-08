namespace FEZEngine.Structure
{
	public class MovementPath
	{
		public List<PathSegment>? Segments;
		public bool NeedsTrigger;
		public PathEndBehavior EndBehavior;
		public string SoundName = "";
		public bool IsSpline;
		public float OffsetSeconds;
		public bool SaveTrigger;
	}
}