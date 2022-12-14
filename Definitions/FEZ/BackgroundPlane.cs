using FEZRepacker.XNB.Attributes;
using System.Drawing;
using System.Numerics;

namespace FEZEngine.Structure
{
	[XNBType("FezEngine.Readers.BackgroundPlaneReader")]
	public class BackgroundPlane
	{
		[XNBProperty]
		public Vector3 Position { get; set; }

		[XNBProperty]
		public Quaternion Rotation { get; set; }

		[XNBProperty]
		public Vector3 Scale { get; set; }

		[XNBProperty]
		public Vector3 Size { get; set; }

		[XNBProperty]
		public string TextureName { get; set; }

		[XNBProperty]
		public bool LightMap { get; set; }

		[XNBProperty]
		public bool AllowOverbrightness { get; set; }
		
		[XNBProperty]
		public Color Filter { get; set; }
		
		[XNBProperty]
		public bool Animated { get; set; }
		
		[XNBProperty]
		public bool Doublesided { get; set; }
		
		[XNBProperty]
		public float Opacity { get; set; }
		
		[XNBProperty(Optional = true)]
		public int? AttachedGroup { get; set; }
		
		[XNBProperty]
		public bool Billboard { get; set; }
		
		[XNBProperty]
		public bool SyncWithSamples { get; set; }
		
		[XNBProperty]
		public bool Crosshatch { get; set; }

		// no idea what it's for. It's in the original reader, so I'm leaving it here
		// perhaps a deleted flag?
		[XNBProperty]
		public bool UnusedFlag { get; set; }

		[XNBProperty]
		public bool AlwaysOnTop { get; set; }
		
		[XNBProperty]
		public bool Fullbright { get; set; }
		
		[XNBProperty]
		public bool PixelatedLightmap { get; set; }
		
		[XNBProperty]
		public bool XTextureRepeat { get; set; }
		
		[XNBProperty]
		public bool YTextureRepeat { get; set; }
		
		[XNBProperty]
		public bool ClampTexture { get; set; }
		
		[XNBProperty(UseConverter = true)]
		public ActorType ActorType { get; set; }
		
		[XNBProperty(Optional = true)]
		public int? AttachedPlane { get; set; }

		[XNBProperty] 
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
