using Microsoft.Xna.Framework.Graphics;

namespace FezEngine.Content
{
    class AnimatedTexture
    {
        public Texture2D Texture { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public List<FrameContent> Frames { get; set; }


        public AnimatedTexture()
        {
            Texture = new();
            Frames = new();
        }
    }
}
