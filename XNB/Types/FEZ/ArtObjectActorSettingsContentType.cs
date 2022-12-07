using System.Numerics;

using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;
using FEZRepacker.Dependencies;

namespace FEZRepacker.XNB.Types.FEZ
{
    class ArtObjectActorSettingsContentType : XNBContentType<ArtObjectActorSettings>
    {
        public ArtObjectActorSettingsContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.ArtObjectActorSettingsReader";

        public override object Read(BinaryReader reader)
        {
            ArtObjectActorSettings settings = new ArtObjectActorSettings();

            settings.Inactive = reader.ReadBoolean();
            settings.ContainedTrile = Converter.ReadType<ActorType>(reader);
            if (reader.ReadBoolean()) settings.AttachedGroup = reader.ReadInt32();
            settings.SpinView = Converter.ReadType<Viewpoint>(reader);
            settings.SpinEvery = reader.ReadSingle();
            settings.SpinOffset = reader.ReadSingle();
            settings.OffCenter = reader.ReadBoolean();
            settings.RotationCenter = reader.ReadVector3();
            settings.VibrationPattern = Converter.ReadType<VibrationMotor[]>(reader);
            settings.CodePattern = Converter.ReadType<CodeInput[]>(reader);
            settings.Segment = Converter.ReadType<PathSegment>(reader);
            if (reader.ReadBoolean()) settings.NextNode = reader.ReadInt32();
            settings.DestinationLevel = Converter.ReadType<string>(reader) ?? "";
            settings.TreasureMapName = Converter.ReadType<string>(reader) ?? "";
            settings.InvisibleSides = Converter.ReadType<FaceOrientation[]>(reader);
            settings.TimeswitchWindBackSpeed = reader.ReadSingle();

            return settings;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            ArtObjectActorSettings settings = (ArtObjectActorSettings)data;

            writer.Write(settings.Inactive);
            Converter.WriteType(settings.ContainedTrile, writer);
            writer.Write(settings.AttachedGroup.HasValue);
            if (settings.AttachedGroup.HasValue) writer.Write(settings.AttachedGroup.GetValueOrDefault());
            Converter.WriteType(settings.SpinView, writer);
            writer.Write(settings.SpinEvery);
            writer.Write(settings.SpinOffset);
            writer.Write(settings.OffCenter);
            writer.Write(settings.RotationCenter);
            Converter.WriteType(settings.VibrationPattern, writer);
            Converter.WriteType(settings.CodePattern, writer);
            Converter.WriteType(settings.Segment, writer);
            writer.Write(settings.NextNode.HasValue);
            if (settings.NextNode.HasValue) writer.Write(settings.NextNode.GetValueOrDefault());
            Converter.WriteType(settings.DestinationLevel, writer);
            Converter.WriteType(settings.TreasureMapName, writer);
            Converter.WriteType(settings.InvisibleSides, writer);
            writer.Write(settings.TimeswitchWindBackSpeed);
        }
    }
}
