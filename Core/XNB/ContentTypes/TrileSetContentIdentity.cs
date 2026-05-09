
using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class TrileSetContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new TrileSetContentSerializer(),
            new DictionaryContentSerializer<int, Trile>(true, false),
            new Int32ContentSerializer(),
            new TrileContentSerializer(),
            new DictionaryContentSerializer<FaceOrientation, CollisionType>(true, true),
            new EnumContentSerializer<FaceOrientation>(),
            new EnumContentSerializer<CollisionType>(),
            new IndexedPrimitivesContentSerializer<VertexInstance, Vector4>(),
            new VertexInstanceContentSerializer(),
            new Vector4ContentSerializer(),
            new EnumContentSerializer<PrimitiveType>(),
            new ArrayContentSerializer<VertexInstance>(),
            new ArrayContentSerializer<ushort>(),
            new UInt16ContentSerializer(),
            new EnumContentSerializer<ActorType>(),
            new EnumContentSerializer<SurfaceType>(),
            new Texture2DContentSerializer(),
            new EnumContentSerializer<SurfaceFormat>().MarkPrivate(),
            new ByteArrayContentSerializer().MarkPrivate()
        };
    }
}
