
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class SpriteFontContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new SpriteFontContentSerializer(),
            new Texture2DContentSerializer(),
            new EnumContentSerializer<SurfaceFormat>().MarkPrivate(),
            new ByteArrayContentSerializer().MarkPrivate(),
            new ListContentSerializer<Rectangle>(true),
            new RectangleContentSerializer(),
            new ListContentSerializer<char>(true),
            new CharContentSerializer(),
            new ListContentSerializer<Vector3>(true),
            new Vector3ContentSerializer(),
        };
    }
}
