using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.TrileFace")]
    [XnbReaderType("FezEngine.Readers.TrileFaceReader")]
    public class TrileFace
    {
        [XnbProperty(UseConverter = true)]
        public TrileEmplacement Id { get; set; }

        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }


        public TrileFace()
        {
            Id = new();
        }
    }
}
