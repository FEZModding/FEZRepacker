
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
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<TrileSet>(),
            new DictionaryContentSerializer<int, Trile>(),
            new Int32ContentSerializer(),
            new GenericContentSerializer<Trile>(),
            new DictionaryContentSerializer<FaceOrientation, CollisionType>(true, true),
            new EnumContentSerializer<FaceOrientation>(),
            new EnumContentSerializer<CollisionType>(),
            new GenericContentSerializer<IndexedPrimitives<VertexInstance, Vector4>>(),
            new GenericContentSerializer<VertexInstance>(),
            new GenericContentSerializer<Vector4>(),
            new EnumContentSerializer<PrimitiveType>(),
            new ArrayContentSerializer<VertexInstance>(),
            new ArrayContentSerializer<ushort>(),
            new UInt16ContentSerializer(),
            new EnumContentSerializer<ActorType>(),
            new EnumContentSerializer<SurfaceType>(),
            new GenericContentSerializer<Texture2D>(),
            new EnumContentSerializer<SurfaceFormat>(),
            new ByteArrayContentSerializer()
        };
    }
}
