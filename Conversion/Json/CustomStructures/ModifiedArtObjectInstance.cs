using FEZRepacker.Definitions.FezEngine;
using FEZRepacker.Definitions.FezEngine.Structure;
using FEZRepacker.Definitions.FezEngine.Structure.Input;
using System.Linq;
using System.Numerics;

namespace FEZRepacker.Conversion.Json.CustomStructures
{
    // Every art object has actor settings anyway, so
    // I'm just making them a part of the art object structure
    class ModifiedArtObjectInstance
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public bool Inactive { get; set; }
        public int? AttachedGroup { get; set; }
        public Vector3 RotationCenter { get; set; }
        public Viewpoint SpinView { get; set; }
        public float SpinEvery { get; set; }
        public float SpinOffset { get; set; }
        public bool OffCenter { get; set; }
        public VibrationMotor[] VibrationPattern { get; set; }
        public CodeInput[] CodePattern { get; set; }
        public PathSegment Segment { get; set; }
        public int? NextNode { get; set; }
        public string DestinationLevel { get; set; }
        public string TreasureMapName { get; set; }
        public FaceOrientation[] InvisibleSides { get; set; }
        public float TimeswitchWindBackSpeed { get; set; }
        public ActorType ContainedTrile { get; set; }

        public ModifiedArtObjectInstance()
        {
            Name = "";
            VibrationPattern = new VibrationMotor[0];
            CodePattern = new CodeInput[0];
            Segment = new();
            DestinationLevel = "";
            TreasureMapName = "";
            InvisibleSides = new FaceOrientation[0];
        }

        public ModifiedArtObjectInstance(ArtObjectInstance artObject)
        {
            // copy over unchanged parameters 
            Name = artObject.Name;
            Position = artObject.Position;
            Rotation = artObject.Rotation;
            Scale = artObject.Scale;

            // copy actor settings
            Inactive = artObject.ActorSettings.Inactive;
            AttachedGroup = artObject.ActorSettings.AttachedGroup;
            RotationCenter = artObject.ActorSettings.RotationCenter;
            SpinView = artObject.ActorSettings.SpinView;
            SpinEvery = artObject.ActorSettings.SpinEvery;
            OffCenter = artObject.ActorSettings.OffCenter;
            VibrationPattern = artObject.ActorSettings.VibrationPattern;
            CodePattern = artObject.ActorSettings.CodePattern;
            Segment = artObject.ActorSettings.Segment;
            NextNode = artObject.ActorSettings.NextNode;
            DestinationLevel = artObject.ActorSettings.DestinationLevel;
            TreasureMapName = artObject.ActorSettings.TreasureMapName;
            InvisibleSides = artObject.ActorSettings.InvisibleSides;
            TimeswitchWindBackSpeed = artObject.ActorSettings.TimeswitchWindBackSpeed;
            ContainedTrile = artObject.ActorSettings.ContainedTrile;
            
        }

        public ArtObjectInstance ToOriginal()
        {
            ArtObjectInstance artObject = new ArtObjectInstance();

            // copy over unchanged parameters 
            artObject.Name = Name;
            artObject.Position = Position;
            artObject.Rotation = Rotation;
            artObject.Scale = Scale;

            // copy actor settings
            artObject.ActorSettings.Inactive = Inactive;
            artObject.ActorSettings.AttachedGroup = AttachedGroup;
            artObject.ActorSettings.RotationCenter = RotationCenter;
            artObject.ActorSettings.SpinView = SpinView;
            artObject.ActorSettings.SpinEvery = SpinEvery;
            artObject.ActorSettings.OffCenter = OffCenter;
            artObject.ActorSettings.VibrationPattern = VibrationPattern;
            artObject.ActorSettings.CodePattern = CodePattern;
            artObject.ActorSettings.Segment = Segment;
            artObject.ActorSettings.NextNode = NextNode;
            artObject.ActorSettings.DestinationLevel = DestinationLevel;
            artObject.ActorSettings.TreasureMapName = TreasureMapName;
            artObject.ActorSettings.InvisibleSides = InvisibleSides;
            artObject.ActorSettings.TimeswitchWindBackSpeed = TimeswitchWindBackSpeed;
            artObject.ActorSettings.ContainedTrile = ContainedTrile;

            return artObject;
        }
    }
}
