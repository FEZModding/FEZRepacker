namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.NpcActionContent")]
    [XnbReaderType("FezEngine.Readers.NpcActionContentReader")]
    public class NpcActionContent
    {
        [XnbProperty(UseConverter = true)]
        public string AnimationName { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string SoundName { get; set; } = "";
    }
}
