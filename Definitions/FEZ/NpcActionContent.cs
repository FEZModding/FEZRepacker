using FEZRepacker.XNB.Attributes;

namespace FEZRepacker.Definitions.FezEngine.Structure
{
	[XNBType("FezEngine.Readers.NpcActionContentReader")]
	public class NpcActionContent
	{
		[XNBProperty(UseConverter = true)]
		public string AnimationName { get; set; }

		[XNBProperty(UseConverter = true)]
		public string SoundName { get; set; }

		
		public NpcActionContent()
        {
			AnimationName = "";
			SoundName = "";
        }
	}
}
