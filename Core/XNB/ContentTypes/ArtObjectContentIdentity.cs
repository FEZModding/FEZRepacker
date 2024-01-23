
using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class ArtObjectContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<ArtObject>(),
            new GenericContentSerializer<Texture2D>(),
            new EnumContentSerializer<SurfaceFormat>(),
            new ByteArrayContentSerializer(),
            new GenericContentSerializer<IndexedPrimitives<VertexInstance, Matrix>>(),
            new GenericContentSerializer<VertexInstance>(),
            new GenericContentSerializer<Matrix>(),
            new EnumContentSerializer<PrimitiveType>(),
            new Int32ContentSerializer(),
            new ArrayContentSerializer<VertexInstance>(),
            new ArrayContentSerializer<ushort>(),
            new UInt16ContentSerializer(),
            new EnumContentSerializer<ActorType>()
        };
    }
}
