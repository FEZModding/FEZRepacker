namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.NpcActionContent, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.NpcActionContentReader, FezEngine")]
    public class NpcActionContent
    {
        [XnbProperty(UseConverter = true)]
        public string AnimationName { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string SoundName { get; set; } = "";
    }
}
