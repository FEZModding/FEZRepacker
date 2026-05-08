using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

using FEZRepacker.Core.Definitions.Game.Graphics;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class AnimatedTextureContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new AnimatedTextureContentSerializer(),
            new ByteArrayContentSerializer().MarkPrivate(),
            new ListContentSerializer<FrameContent>(),
            new FrameContentContentSerializer(),
            new TimeSpanContentSerializer(),
            new RectangleContentSerializer()
        };
    }
}
