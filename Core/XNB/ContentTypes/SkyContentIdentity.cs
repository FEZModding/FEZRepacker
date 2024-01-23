

using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class SkyContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<Sky>(),
            new ListContentSerializer<SkyLayer>(),
            new GenericContentSerializer<SkyLayer>(),
            new ListContentSerializer<string>(),
            new StringContentSerializer()
        };
    }
}
