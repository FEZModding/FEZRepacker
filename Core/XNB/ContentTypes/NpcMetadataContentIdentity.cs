
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class NpcMetadataContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<NpcMetadata>(),
            new StringContentSerializer(),
            new ListContentSerializer<NpcAction>(true),
            new EnumContentSerializer<NpcAction>(),
            new Int32ContentSerializer()
        };
    }
}
