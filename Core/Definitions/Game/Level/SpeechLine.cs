namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.SpeechLine, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.SpeechLineReader, FezEngine")]
    public class SpeechLine
    {
        [XnbProperty(UseConverter = true)]
        public string Text { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public NpcActionContent OverrideContent { get; set; } = new();
    }
}