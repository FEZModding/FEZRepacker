using Microsoft.Xna.Framework.Graphics;

namespace FezEngine.Structure
{
    class AnimatedTexture
    {
        public Texture2D Texture = new Texture2D();
        public int FrameWidth;
        public int FrameHeight;
        public List<FrameContent> Frames = new List<FrameContent>();
    }
}
