
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class TextureContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new Texture2DContentSerializer(),
            new EnumContentSerializer<SurfaceFormat>().MarkPrivate(),
            new ByteArrayContentSerializer().MarkPrivate()
        };
    }
}
