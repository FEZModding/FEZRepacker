
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Graphics
{
    [XnbType("FezEngine.Content.FrameContent")]
    [XnbReaderType("FezEngine.Readers.FrameReader")]
    public class FrameContent
    {
        [XnbProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; }

        [XnbProperty(UseConverter = true)]
        public Rectangle Rectangle { get; set; }
    }
}
