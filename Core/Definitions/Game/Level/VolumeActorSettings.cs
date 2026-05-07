using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.VolumeActorSettings, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.VolumeActorSettingsReader, FezEngine")]
    public class VolumeActorSettings
    {
        [XnbProperty]
        public Vector2 FarawayPlaneOffset { get; set; }

        [XnbProperty]
        public bool IsPointOfInterest { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<DotDialogueLine> DotDialogue { get; set; } = new();

        [XnbProperty]
        public bool WaterLocked { get; set; }

        [XnbProperty(UseConverter = true)]
        public CodeInput[] CodePattern { get; set; } = { };

        [XnbProperty]
        public bool IsBlackHole { get; set; }

        [XnbProperty]
        public bool NeedsTrigger { get; set; }

        [XnbProperty]
        public bool IsSecretPassage { get; set; }
    }
}
