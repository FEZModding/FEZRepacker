
using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class ArtObjectContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new ArtObjectContentSerializer(),
            new Texture2DContentSerializer(),
            new EnumContentSerializer<SurfaceFormat>().MarkPrivate(),
            new ByteArrayContentSerializer().MarkPrivate(),
            new IndexedPrimitivesContentSerializer<VertexInstance, Matrix>(),
            new VertexInstanceContentSerializer(),
            new MatrixContentSerializer(),
            new EnumContentSerializer<PrimitiveType>(),
            new Int32ContentSerializer(),
            new ArrayContentSerializer<VertexInstance>(),
            new ArrayContentSerializer<ushort>(),
            new UInt16ContentSerializer(),
            new EnumContentSerializer<ActorType>()
        };
    }
}
