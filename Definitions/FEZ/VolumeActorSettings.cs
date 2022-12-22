using FezEngine.Structure.Input;
using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FezEngine.Structure
{
    [XNBType("FezEngine.Readers.VolumeActorSettingsReader")]
    class VolumeActorSettings
    {
        [XNBProperty]
        public Vector2 FarawayPlaneOffset { get; set; }

        [XNBProperty]
        public bool IsPointOfInterest { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<DotDialogueLine> DotDialogue { get; set; }

        [XNBProperty]
        public bool WaterLocked { get; set; }

        [XNBProperty(UseConverter = true)]
        public CodeInput[] CodePattern { get; set; }

        [XNBProperty]
        public bool IsBlackHole { get; set; }

        [XNBProperty]
        public bool NeedsTrigger { get; set; }

        [XNBProperty]
        public bool IsSecretPassage { get; set; }


        public VolumeActorSettings()
        {
            DotDialogue = new List<DotDialogueLine>();
            CodePattern = new CodeInput[0];
        }
    }
}
