using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;
using System.Linq;
using System.Numerics;

namespace FEZRepacker.Conversion.Json.CustomStructures
{
    // Every volume has actor settings anyway, and since they're rare,
    // I'm just making them a part of the volume structure
    class ModifiedVolume
    {
        public FaceOrientation[] Orientations { get; set; }
        public Vector3 From { get; set; }
        public Vector3 To { get; set; }
        public List<DotDialogueLine> DotDialogue { get; set; }
        public CodeInput[] CodePattern { get; set; }
        public bool IsBlackHole { get; set; }
        public bool NeedsTrigger { get; set; }
        public bool IsSecretPassage { get; set; }
        public bool WaterLocked { get; set; }
        public bool IsPointOfInterest { get; set; }
        public Vector2 FarawayPlaneOffset { get; set; }

        public ModifiedVolume()
        {
            Orientations = new FaceOrientation[0];
            DotDialogue = new();
            CodePattern = new CodeInput[0];
        }

        public ModifiedVolume(Volume volume)
        {
            // copy over unchanged parameters 
            Orientations = volume.Orientations;
            From = volume.From;
            To = volume.To;

            // copy actor settings
            FarawayPlaneOffset = volume.ActorSettings.FarawayPlaneOffset;
            IsPointOfInterest = volume.ActorSettings.IsPointOfInterest;
            DotDialogue = volume.ActorSettings.DotDialogue;
            WaterLocked = volume.ActorSettings.WaterLocked;
            CodePattern = volume.ActorSettings.CodePattern;
            IsBlackHole = volume.ActorSettings.IsBlackHole;
            NeedsTrigger = volume.ActorSettings.NeedsTrigger;
            IsSecretPassage = volume.ActorSettings.IsSecretPassage;
        }

        public Volume ToOriginal()
        {
            Volume volume = new Volume();

            // copy over original values
            volume.Orientations = Orientations;
            volume.From = From;
            volume.To = To;

            // copy actor settings
            volume.ActorSettings.FarawayPlaneOffset = FarawayPlaneOffset;
            volume.ActorSettings.IsPointOfInterest = IsPointOfInterest;
            volume.ActorSettings.DotDialogue = DotDialogue;
            volume.ActorSettings.WaterLocked = WaterLocked;
            volume.ActorSettings.CodePattern = CodePattern;
            volume.ActorSettings.IsBlackHole = IsBlackHole;
            volume.ActorSettings.NeedsTrigger = IsSecretPassage;
            volume.ActorSettings.IsSecretPassage = IsSecretPassage;

            return volume;
        }
    }
}
