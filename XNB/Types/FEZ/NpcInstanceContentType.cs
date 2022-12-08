using FEZEngine;
using FEZEngine.Structure;
using FEZRepacker.Dependencies;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class NpcInstanceContentType : XNBContentType<NpcInstance>
    {
        public NpcInstanceContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.NpcInstanceReader";

        public override object Read(BinaryReader reader)
        {
            NpcInstance npc = new NpcInstance();

            npc.Name = reader.ReadString();
            npc.Position = reader.ReadVector3();
            npc.DestinationOffset = reader.ReadVector3();
            npc.WalkSpeed = reader.ReadSingle();
            npc.RandomizeSpeech = reader.ReadBoolean();
            npc.SayFirstSpeechLineOnce = reader.ReadBoolean();
            npc.AvoidsGomez = reader.ReadBoolean();
            npc.ActorType = Converter.ReadType<ActorType>(reader);
            npc.Speech = Converter.ReadType<List<SpeechLine>>(reader) ?? npc.Speech;
            npc.Actions = Converter.ReadType<Dictionary<NpcAction, NpcActionContent>>(reader) ?? npc.Actions;

            return npc;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            NpcInstance npc = (NpcInstance)data;

            writer.Write(npc.Name);
            writer.Write(npc.Position);
            writer.Write(npc.DestinationOffset);
            writer.Write(npc.WalkSpeed);
            writer.Write(npc.RandomizeSpeech);
            writer.Write(npc.SayFirstSpeechLineOnce);
            writer.Write(npc.AvoidsGomez);
            Converter.WriteType(npc.ActorType, writer);
            Converter.WriteType(npc.Speech, writer);
            Converter.WriteType(npc.Actions, writer);

        }
    }
}
