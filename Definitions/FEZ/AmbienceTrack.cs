using FEZRepacker.XNB.Attributes;

namespace FezEngine.Structure
{
	[XNBType("FezEngine.Readers.AmbienceTrackReader")]
	public class AmbienceTrack
	{
		[XNBProperty(UseConverter = true)]
		public string Name { get; set; }

		[XNBProperty] 
		public bool Day { get; set; }
		
		[XNBProperty]
		public bool Dusk { get; set; }
		
		[XNBProperty]
		public bool Night { get; set; }
		
		[XNBProperty]
		public bool Dawn { get; set; }


		public AmbienceTrack()
        {
			Name = "";
        }
	}
}
