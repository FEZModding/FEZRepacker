using System.Numerics;

namespace FEZEngine.Structure
{
	public class NpcInstance
	{
		public string Name = "";
		public Vector3 Position;
		public Vector3 DestinationOffset;
		public float WalkSpeed;
		public bool RandomizeSpeech;
		public bool SayFirstSpeechLineOnce;
		public bool AvoidsGomez;
		public ActorType ActorType;
		public List<SpeechLine> Speech = new();
		public Dictionary<NpcAction, NpcActionContent> Actions = new();
	}
}