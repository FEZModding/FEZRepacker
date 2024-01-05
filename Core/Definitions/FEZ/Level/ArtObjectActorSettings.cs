using System.Numerics;

using FEZRepacker.Converter.Definitions.FezEngine.Structure.Input;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.ArtObjectActorSettings")]
    [XnbReaderType("FezEngine.Readers.ArtObjectActorSettingsReader")]
    internal class ArtObjectActorSettings
    {
        [XnbProperty]
        public bool Inactive { get; set; }

        [XnbProperty(UseConverter = true)]
        public ActorType ContainedTrile { get; set; }

        [XnbProperty(Optional = true)]
        public int? AttachedGroup { get; set; }

        [XnbProperty(UseConverter = true)]
        public Viewpoint SpinView { get; set; }

        [XnbProperty]
        public float SpinEvery { get; set; }

        [XnbProperty]
        public float SpinOffset { get; set; }

        [XnbProperty]
        public bool OffCenter { get; set; }

        [XnbProperty]
        public Vector3 RotationCenter { get; set; }

        [XnbProperty(UseConverter = true)]
        public VibrationMotor[] VibrationPattern { get; set; }

        [XnbProperty(UseConverter = true)]
        public CodeInput[] CodePattern { get; set; }

        [XnbProperty(UseConverter = true)]
        public PathSegment Segment { get; set; }

        [XnbProperty(Optional = true)]
        public int? NextNode { get; set; }

        [XnbProperty(UseConverter = true)]
        public string DestinationLevel { get; set; }

        [XnbProperty(UseConverter = true)]
        public string TreasureMapName { get; set; }

        [XnbProperty(UseConverter = true)]
        public FaceOrientation[] InvisibleSides { get; set; }

        [XnbProperty]
        public float TimeswitchWindBackSpeed { get; set; }


        public ArtObjectActorSettings()
        {
            VibrationPattern = new VibrationMotor[0];
            CodePattern = new CodeInput[0];
            Segment = new();
            DestinationLevel = "";
            TreasureMapName = "";
            InvisibleSides = new FaceOrientation[0];
        }
    }
}
