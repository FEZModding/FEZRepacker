using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class MapContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new MapTreeContentSerializer(),
            new MapNodeContentSerializer(),
            new ListContentSerializer<MapNodeConnection>(),
            new MapNodeConnectionContentSerializer(),
            new EnumContentSerializer<FaceOrientation>(),
            new Int32ContentSerializer(),
            new EnumContentSerializer<LevelNodeType>(),
            new WinConditionsContentSerializer(),
            new ListContentSerializer<int>()
        };
    }
}
