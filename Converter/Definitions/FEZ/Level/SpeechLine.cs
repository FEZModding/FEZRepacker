namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
	[XnbType("FezEngine.Structure.SpeechLine")]
	[XnbReaderType("FezEngine.Readers.SpeechLineReader")]
	internal class SpeechLine
	{
		[XnbProperty(UseConverter = true)]
		public string Text { get; set; }

		[XnbProperty(UseConverter = true)]
		public NpcActionContent OverrideContent { get; set; }


		public SpeechLine()
        {
			Text = "";
			OverrideContent = new();
        }
	}
}