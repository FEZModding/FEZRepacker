using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class TrileInstanceActorSettingsContentType : XNBContentType<TrileInstanceActorSettings>
    {
        public TrileInstanceActorSettingsContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.InstanceActorSettingsReader";

        public override object Read(BinaryReader reader)
        {
            var settings = new TrileInstanceActorSettings();
            if (reader.ReadBoolean()) settings.ContainedTrile = reader.ReadInt32();
            settings.SignText = Converter.ReadType<string>(reader) ?? settings.SignText;
            settings.Sequence = Converter.ReadType<bool[]>(reader) ?? settings.Sequence;
            settings.SequenceSampleName = Converter.ReadType<string>(reader) ?? settings.SequenceSampleName;
            settings.SequenceAlternateSampleName = Converter.ReadType<string>(reader) ?? settings.SequenceAlternateSampleName;
            if (reader.ReadBoolean()) settings.HostVolume = reader.ReadInt32();

            return settings;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileInstanceActorSettings settings = (TrileInstanceActorSettings)data;

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
