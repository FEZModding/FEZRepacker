using FEZRepacker.Converter.Definitions.MicrosoftXna;

namespace FEZRepacker.Converter.Definitions.FezEngine.Content
{
    [XnbType("FezEngine.Content.FrameContent")]
    [XnbReaderType("FezEngine.Readers.FrameReader")]
    internal class FrameContent
    {
        [XnbProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; }

        [XnbProperty(UseConverter = true)]
        public Rectangle Rectangle { get; set; }
    }
}
