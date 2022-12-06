using FEZEngine.Structure.Input;
using System.ComponentModel;
using System.Numerics;

namespace FEZEngine.Structure
{
    class VolumeActorSettings
    {
        public Vector2 FarawayPlaneOffset;
        public bool IsPointOfInterest;
        public List<DotDialogueLine>? DotDialogue;
        public bool WaterLocked;
        public CodeInput[]? CodePattern;
        public bool IsBlackHole;
        public bool NeedsTrigger;
        public bool IsSecretPassage;
    }
}
