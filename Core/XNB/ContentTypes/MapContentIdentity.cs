using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class MapContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<MapTree>(),
            new GenericContentSerializer<MapNode>(),
            new ListContentSerializer<MapNodeConnection>(),
            new GenericContentSerializer<MapNodeConnection>(),
            new EnumContentSerializer<FaceOrientation>(),
            new Int32ContentSerializer(),
            new EnumContentSerializer<LevelNodeType>(),
            new GenericContentSerializer<WinConditions>(),
            new ListContentSerializer<int>()
        };
    }
}
