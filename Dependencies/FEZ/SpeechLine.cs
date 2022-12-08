namespace FEZEngine.Structure
{
	public class SpeechLine
	{
		public string Text { get; set; }
		public NpcActionContent OverrideContent { get; set; }


		public SpeechLine()
        {
			Text = "";
			OverrideContent = new();
        }
	}
}