namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.SpeechLine")]
    [XnbReaderType("FezEngine.Readers.SpeechLineReader")]
    public class SpeechLine
    {
        [XnbProperty(UseConverter = true)]
        public string Text { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public NpcActionContent OverrideContent { get; set; } = new();
    }
}