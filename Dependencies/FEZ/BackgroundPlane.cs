using System.Drawing;
using System.Numerics;

namespace FEZEngine.Structure
{
	public class BackgroundPlane
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
		public Vector3 Size;
		public string TextureName = "";
		public bool LightMap;
		public bool AllowOverbrightness;
		public Color Filter;
		public bool Animated;
		public bool Doublesided;
		public float Opacity;
		public int? AttachedGroup;
		public bool Billboard;
		public bool SyncWithSamples;
		public bool Crosshatch;
		public bool AlwaysOnTop;
		public bool Fullbright;
		public bool PixelatedLightmap;
		public bool XTextureRepeat;
		public bool YTextureRepeat;
		public bool ClampTexture;
		public ActorType ActorType;
		public int? AttachedPlane;
		public float ParallaxFactor;
	}
}
