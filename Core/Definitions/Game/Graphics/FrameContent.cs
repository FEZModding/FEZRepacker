
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Graphics
{
    [XnbType("FezEngine.Content.FrameContent, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.FrameReader, FezEngine")]
    public class FrameContent
    {
        [XnbProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; }

        [XnbProperty(UseConverter = true)]
        public Rectangle Rectangle { get; set; } = new();
    }
}
