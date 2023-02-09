namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.NpcActionContent")]
    [XnbReaderType("FezEngine.Readers.NpcActionContentReader")]
    internal class NpcActionContent
    {
        [XnbProperty(UseConverter = true)]
        public string AnimationName { get; set; }

        [XnbProperty(UseConverter = true)]
        public string SoundName { get; set; }


        public NpcActionContent()
        {
            AnimationName = "";
            SoundName = "";
        }
    }
}