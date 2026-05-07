namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.DotDialogueLine, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.DotDialogueLineReader, FezEngine")]
    public class DotDialogueLine
    {
        [XnbProperty(UseConverter = true)]
        public string ResourceText { get; set; } = "";

        [XnbProperty]
        public bool Grouped { get; set; }
    }
}
