

using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class SoundEffectContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<SoundEffect>(),
            new ByteArrayContentSerializer()
        };
        
    }
}
