namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
	[XnbType("FezEngine.Readers.CameraNodeDataReader")]
	internal class CameraNodeData
	{
		[XnbProperty]
		public bool Perspective { get; set; }

		[XnbProperty]
		public int PixelsPerTrixel { get; set; }

		[XnbProperty(UseConverter = true)]
		public string SoundName { get; set; }


		public CameraNodeData()
        {
			SoundName = "";

		}
	}
}
