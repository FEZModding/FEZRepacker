using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

using FEZRepacker.Core.Definitions.Game.Graphics;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class AnimatedTextureContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new AnimatedTextureContentSerializer(),
            new ByteArrayContentSerializer(),
            new ListContentSerializer<FrameContent>(),
            new FrameContentContentSerializer(),
            new TimeSpanContentSerializer(),
            new RectangleContentSerializer()
        };
    }
}
