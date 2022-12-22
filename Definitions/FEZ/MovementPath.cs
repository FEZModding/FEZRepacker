using FEZRepacker.XNB.Attributes;

namespace FezEngine.Structure
{
	[XNBType("FezEngine.Readers.MovementPathReader")]
	public class MovementPath
	{
		[XNBProperty(UseConverter = true)]
		public List<PathSegment> Segments { get; set; }

		[XNBProperty] 
		public bool NeedsTrigger { get; set; }
		
		[XNBProperty(UseConverter = true)]
		public PathEndBehavior EndBehavior { get; set; }
		
		[XNBProperty(UseConverter = true)]
		public string SoundName { get; set; }
		
		[XNBProperty]
		public bool IsSpline { get; set; }
		
		[XNBProperty]
		public float OffsetSeconds { get; set; }
		
		[XNBProperty]
		public bool SaveTrigger { get; set; }

		
		public MovementPath()
        {
			Segments = new();
			SoundName = "";
		}
	}
}