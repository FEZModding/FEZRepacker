using FEZRepacker.Converter.Definitions.FezEngine.Structure.Input;
using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.VolumeActorSettings")]
    [XnbReaderType("FezEngine.Readers.VolumeActorSettingsReader")]
    internal class VolumeActorSettings
    {
        [XnbProperty]
        public Vector2 FarawayPlaneOffset { get; set; }

        [XnbProperty]
        public bool IsPointOfInterest { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<DotDialogueLine> DotDialogue { get; set; }

        [XnbProperty]
        public bool WaterLocked { get; set; }

        [XnbProperty(UseConverter = true)]
        public CodeInput[] CodePattern { get; set; }

        [XnbProperty]
        public bool IsBlackHole { get; set; }

        [XnbProperty]
        public bool NeedsTrigger { get; set; }

        [XnbProperty]
        public bool IsSecretPassage { get; set; }


        public VolumeActorSettings()
        {
            DotDialogue = new List<DotDialogueLine>();
            CodePattern = new CodeInput[0];
        }
    }
}
