using System.Drawing;
using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
	[XnbType("FezEngine.Readers.BackgroundPlaneReader")]
	internal class BackgroundPlane
	{
		[XnbProperty]
		public Vector3 Position { get; set; }

		[XnbProperty]
		public Quaternion Rotation { get; set; }

		[XnbProperty]
		public Vector3 Scale { get; set; }

		[XnbProperty]
		public Vector3 Size { get; set; }

		[XnbProperty]
		public string TextureName { get; set; }

		[XnbProperty]
		public bool LightMap { get; set; }

		[XnbProperty]
		public bool AllowOverbrightness { get; set; }
		
		[XnbProperty]
		public Color Filter { get; set; }
		
		[XnbProperty]
		public bool Animated { get; set; }
		
		[XnbProperty]
		public bool Doublesided { get; set; }
		
		[XnbProperty]
		public float Opacity { get; set; }
		
		[XnbProperty(Optional = true)]
		public int? AttachedGroup { get; set; }
		
		[XnbProperty]
		public bool Billboard { get; set; }
		
		[XnbProperty]
		public bool SyncWithSamples { get; set; }
		
		[XnbProperty]
		public bool Crosshatch { get; set; }

		// no idea what it's for. It's in the original reader, so I'm leaving it here
		// perhaps a deleted flag?
		[XnbProperty]
		public bool UnusedFlag { get; set; }

		[XnbProperty]
		public bool AlwaysOnTop { get; set; }
		
		[XnbProperty]
		public bool Fullbright { get; set; }
		
		[XnbProperty]
		public bool PixelatedLightmap { get; set; }
		
		[XnbProperty]
		public bool XTextureRepeat { get; set; }
		
		[XnbProperty]
		public bool YTextureRepeat { get; set; }
		
		[XnbProperty]
		public bool ClampTexture { get; set; }
		
		[XnbProperty(UseConverter = true)]
		public ActorType ActorType { get; set; }
		
		[XnbProperty(Optional = true)]
		public int? AttachedPlane { get; set; }

		[XnbProperty] 
		public float ParallaxFactor { get; set; }

		
		public BackgroundPlane()
        {
			Scale = new Vector3(1.0f);
			Opacity = 1.0f;
			Filter = Color.White;
			Rotation = Quaternion.Identity;
			TextureName = "";
		}
	}
}
