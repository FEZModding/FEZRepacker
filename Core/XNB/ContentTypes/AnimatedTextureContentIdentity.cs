using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class AnimatedTextureContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<AnimatedTexture>(),
            new ByteArrayContentSerializer(),
            new ListContentSerializer<FrameContent>(),
            new GenericContentSerializer<FrameContent>(),
            new TimeSpanContentSerializer(),
            new GenericContentSerializer<Rectangle>()
        };
    }
}
