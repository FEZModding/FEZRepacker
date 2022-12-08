using System.Drawing;
using System.Numerics;

namespace FEZEngine.Structure
{
	public class BackgroundPlane
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public Vector3 Scale { get; set; }
		public Vector3 Size { get; set; }
		public string TextureName { get; set; }
		public bool LightMap { get; set; }
		public bool AllowOverbrightness { get; set; }
		public Color Filter { get; set; }
		public bool Animated { get; set; }
		public bool Doublesided { get; set; }
		public float Opacity { get; set; }
		public int? AttachedGroup { get; set; }
		public bool Billboard { get; set; }
		public bool SyncWithSamples { get; set; }
		public bool Crosshatch { get; set; }
		public bool AlwaysOnTop { get; set; }
		public bool Fullbright { get; set; }
		public bool PixelatedLightmap { get; set; }
		public bool XTextureRepeat { get; set; }
		public bool YTextureRepeat { get; set; }
		public bool ClampTexture { get; set; }
		public ActorType ActorType { get; set; }
		public int? AttachedPlane { get; set; }
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
