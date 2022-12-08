using System.Numerics;

using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;

namespace FEZRepacker.XNB.Types.FEZ
{
    class VolumeActorSettingsContentType : XNBContentType<VolumeActorSettings>
    {
        public VolumeActorSettingsContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.VolumeActorSettingsReader";

        public override object Read(BinaryReader reader)
        {
            VolumeActorSettings settings = new VolumeActorSettings();

            settings.FarawayPlaneOffset = new Vector2(
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            if (reader.ReadBoolean()) settings.IsPointOfInterest = true;
            settings.DotDialogue = Converter.ReadType<List<DotDialogueLine>>(reader) ?? settings.DotDialogue;
            if(reader.ReadBoolean()) settings.WaterLocked = true;
            settings.CodePattern = Converter.ReadType<CodeInput[]>(reader) ?? settings.CodePattern;
            if (reader.ReadBoolean()) settings.IsBlackHole = true;
            if (reader.ReadBoolean()) settings.NeedsTrigger = true;
            if (reader.ReadBoolean()) settings.IsSecretPassage = true;

            return settings;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            VolumeActorSettings settings = (VolumeActorSettings)data;

            writer.Write(settings.FarawayPlaneOffset.X);
            writer.Write(settings.FarawayPlaneOffset.Y);

            writer.Write(settings.IsPointOfInterest);
            Converter.WriteType(settings.DotDialogue, writer);
            writer.Write(settings.WaterLocked);
            Converter.WriteType(settings.CodePattern, writer);
            writer.Write(settings.IsBlackHole);
            writer.Write(settings.NeedsTrigger);
            writer.Write(settings.IsSecretPassage);
        }
    }
}
