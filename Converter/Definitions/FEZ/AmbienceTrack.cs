namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
	[XnbType("FezEngine.Readers.AmbienceTrackReader")]
	internal class AmbienceTrack
	{
		[XnbProperty(UseConverter = true)]
		public string Name { get; set; }

		[XnbProperty] 
		public bool Day { get; set; }
		
		[XnbProperty]
		public bool Dusk { get; set; }
		
		[XnbProperty]
		public bool Night { get; set; }
		
		[XnbProperty]
		public bool Dawn { get; set; }


		public AmbienceTrack()
        {
			Name = "";
        }
	}
}
