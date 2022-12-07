using System.Numerics;

using FEZEngine;
using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class InstanceActorSettingsContentType : XNBContentType<InstanceActorSettings>
    {
        public InstanceActorSettingsContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.InstanceActorSettingsReader";

        public override object Read(BinaryReader reader)
        {
            var settings = new InstanceActorSettings();
            if (reader.ReadBoolean()) settings.ContainedTrile = reader.ReadInt32();
            settings.SignText = Converter.ReadType<string>(reader) ?? "";
            settings.Sequence = Converter.ReadType<bool[]>(reader) ?? new bool[0];
            settings.SequenceSampleName = Converter.ReadType<string>(reader) ?? "";
            settings.SequenceAlternateSampleName = Converter.ReadType<string>(reader) ?? "";
            if (reader.ReadBoolean()) settings.HostVolume = reader.ReadInt32();

            return settings;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            InstanceActorSettings settings = (InstanceActorSettings)data;

            writer.Write(settings.ContainedTrile.HasValue);
            if (settings.ContainedTrile.HasValue) writer.Write(settings.ContainedTrile.GetValueOrDefault());

            Converter.WriteType(settings.SignText, writer);
            Converter.WriteType(settings.Sequence, writer);
            Converter.WriteType(settings.SequenceSampleName, writer);
            Converter.WriteType(settings.SequenceAlternateSampleName, writer);

            writer.Write(settings.HostVolume.HasValue);
            if (settings.HostVolume.HasValue) writer.Write(settings.HostVolume.GetValueOrDefault());
        }
    }
}
