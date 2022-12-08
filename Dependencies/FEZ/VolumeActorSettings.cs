using FEZEngine.Structure.Input;
using System.Numerics;

namespace FEZEngine.Structure
{
    class VolumeActorSettings
    {
        public Vector2 FarawayPlaneOffset { get; set; }
        public bool IsPointOfInterest { get; set; }
        public List<DotDialogueLine> DotDialogue { get; set; }
        public bool WaterLocked { get; set; }
        public CodeInput[] CodePattern { get; set; }
        public bool IsBlackHole { get; set; }
        public bool NeedsTrigger { get; set; }
        public bool IsSecretPassage { get; set; }


        public VolumeActorSettings()
        {
            DotDialogue = new List<DotDialogueLine>();
            CodePattern = new CodeInput[0];
        }
    }
}
