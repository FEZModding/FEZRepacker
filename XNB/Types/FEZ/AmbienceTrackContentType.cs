using FEZEngine;
using FEZEngine.Structure;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class AmbienceTrackContentType : XNBContentType<AmbienceTrack>
    {
        public AmbienceTrackContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.AmbienceTrackReader";

        public override object Read(BinaryReader reader)
        {
            AmbienceTrack ambience = new AmbienceTrack();

            ambience.Name = Converter.ReadType<string>(reader) ?? "";
            ambience.Dawn = reader.ReadBoolean();
            ambience.Day = reader.ReadBoolean();
            ambience.Dusk = reader.ReadBoolean();
            ambience.Night = reader.ReadBoolean();

            return ambience;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            AmbienceTrack ambience = (AmbienceTrack)data;

            Converter.WriteType(ambience.Name, writer);
            writer.Write(ambience.Dawn);
            writer.Write(ambience.Day);
            writer.Write(ambience.Dusk);
            writer.Write(ambience.Night);
        }
    }
}
