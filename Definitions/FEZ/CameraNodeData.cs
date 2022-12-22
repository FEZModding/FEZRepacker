using FEZRepacker.XNB.Attributes;

namespace FezEngine.Structure
{
	[XNBType("FezEngine.Readers.CameraNodeDataReader")]
	public class CameraNodeData
	{
		[XNBProperty]
		public bool Perspective { get; set; }

		[XNBProperty]
		public int PixelsPerTrixel { get; set; }

		[XNBProperty(UseConverter = true)]
		public string SoundName { get; set; }


		public CameraNodeData()
        {
			SoundName = "";

		}
	}
}
