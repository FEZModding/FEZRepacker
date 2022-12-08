using System.Numerics;

namespace FEZEngine.Structure
{
	public class NpcInstance
	{
		public string Name { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 DestinationOffset { get; set; }
		public float WalkSpeed { get; set; }
		public bool RandomizeSpeech { get; set; }
		public bool SayFirstSpeechLineOnce { get; set; }
		public bool AvoidsGomez { get; set; }
		public ActorType ActorType { get; set; }
		public List<SpeechLine> Speech { get; set; }
		public Dictionary<NpcAction, NpcActionContent> Actions { get; set; }


		public NpcInstance()
        {
			Speech = new List<SpeechLine>();
			Actions = new Dictionary<NpcAction, NpcActionContent>();

			Name = "";
		}
	}
}