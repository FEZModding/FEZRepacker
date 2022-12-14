using FEZRepacker.XNB.Attributes;
using System.Drawing;

namespace FezEngine.Content
{
    [XNBType("FezEngine.Readers.FrameReader")]
    class FrameContent
    {
        [XNBProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; }

        [XNBProperty(UseConverter = true)]
        public Rectangle Rectangle { get; set; }
    }
}
