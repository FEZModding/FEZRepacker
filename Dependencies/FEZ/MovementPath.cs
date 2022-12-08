namespace FEZEngine.Structure
{
	public class MovementPath
	{
		public List<PathSegment> Segments { get; set; }
		public bool NeedsTrigger { get; set; }
		public PathEndBehavior EndBehavior { get; set; }
		public string SoundName { get; set; }
		public bool IsSpline { get; set; }
		public float OffsetSeconds { get; set; }
		public bool SaveTrigger { get; set; }

		
		public MovementPath()
        {
			Segments = new();
			SoundName = "";
		}
	}
}