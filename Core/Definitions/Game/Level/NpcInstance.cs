using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.NpcInstance, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.NpcInstanceReader, FezEngine")]
    public class NpcInstance
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [XnbProperty]
        public Vector3 Position { get; set; }

        [XnbProperty]
        public Vector3 DestinationOffset { get; set; }

        [XnbProperty]
        public float WalkSpeed { get; set; }

        [XnbProperty]
        public bool RandomizeSpeech { get; set; }

        [XnbProperty]
        public bool SayFirstSpeechLineOnce { get; set; }

        [XnbProperty]
        public bool AvoidsGomez { get; set; }

        [XnbProperty(UseConverter = true)]
        public ActorType ActorType { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<SpeechLine> Speech { get; set; } = null!;

        [XnbProperty(UseConverter = true)]
        public IDictionary<NpcAction, NpcActionContent> Actions { get; set; } = null!;
    }
}