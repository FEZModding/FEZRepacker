using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FEZEngine.Structure
{
	[XNBType("FezEngine.Readers.NpcInstanceReader")]
	public class NpcInstance
	{
		[XNBProperty]
		public string Name { get; set; }

		[XNBProperty] 
		public Vector3 Position { get; set; }
		
		[XNBProperty]
		public Vector3 DestinationOffset { get; set; }
		
		[XNBProperty]
		public float WalkSpeed { get; set; }
		
		[XNBProperty]
		public bool RandomizeSpeech { get; set; }
		
		[XNBProperty]
		public bool SayFirstSpeechLineOnce { get; set; }
		
		[XNBProperty]
		public bool AvoidsGomez { get; set; }
		
		[XNBProperty(UseConverter = true)]
		public ActorType ActorType { get; set; }
		
		[XNBProperty(UseConverter = true)]
		public List<SpeechLine> Speech { get; set; }
		
		[XNBProperty(UseConverter = true)]
		public Dictionary<NpcAction, NpcActionContent> Actions { get; set; }


		public NpcInstance()
        {
			Speech = new List<SpeechLine>();
			Actions = new Dictionary<NpcAction, NpcActionContent>();

			Name = "";
		}
	}
}