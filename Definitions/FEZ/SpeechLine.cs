using FEZRepacker.XNB.Attributes;

namespace FEZRepacker.Definitions.FezEngine.Structure
{
	[XNBType("FezEngine.Readers.SpeechLineReader")]
	public class SpeechLine
	{
		[XNBProperty(UseConverter = true)]
		public string Text { get; set; }

		[XNBProperty(UseConverter = true)]
		public NpcActionContent OverrideContent { get; set; }


		public SpeechLine()
        {
			Text = "";
			OverrideContent = new();
        }
	}
}