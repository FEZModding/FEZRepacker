namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.DotDialogueLine")]
    [XnbReaderType("FezEngine.Readers.DotDialogueLineReader")]
    public class DotDialogueLine
    {
        [XnbProperty(UseConverter = true)]
        public string ResourceText { get; set; }

        [XnbProperty]
        public bool Grouped { get; set; }


        public DotDialogueLine()
        {
            ResourceText = "";
        }
    }
}
