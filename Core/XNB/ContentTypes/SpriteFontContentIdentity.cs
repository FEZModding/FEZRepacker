
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class SpriteFontContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new SpriteFontContentSerializer(),
            new Texture2DContentSerializer(),
            new EnumContentSerializer<SurfaceFormat>(),
            new ByteArrayContentSerializer(),
            new ListContentSerializer<Rectangle>(true),
            new RectangleContentSerializer(),
            new ListContentSerializer<char>(),
            new CharContentSerializer(),
            new ListContentSerializer<Vector3>(true),
            new Vector3ContentSerializer(),
        };
    }
}
