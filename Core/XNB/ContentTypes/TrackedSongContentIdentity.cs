
using FEZRepacker.Core.Definitions.Game.TrackedSong;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class TrackedSongContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<TrackedSong>(),
            new ListContentSerializer<Loop>(),
            new GenericContentSerializer<Loop>(),
            new ArrayContentSerializer<ShardNotes>(),
            new EnumContentSerializer<ShardNotes>(),
            new Int32ContentSerializer(),
            new EnumContentSerializer<AssembleChords>()
        };
    }
}
