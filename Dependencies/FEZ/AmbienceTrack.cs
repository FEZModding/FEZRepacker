namespace FEZEngine.Structure
{
	public class AmbienceTrack
	{
		public string Name { get; set; }
		public bool Day { get; set; }
		public bool Dusk { get; set; }
		public bool Night { get; set; }
		public bool Dawn { get; set; }


		public AmbienceTrack()
        {
			Name = "";
        }
	}
}
