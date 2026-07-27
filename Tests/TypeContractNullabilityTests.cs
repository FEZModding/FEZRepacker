using System.Reflection;
using System.Text.Json;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.Definitions.Game.TrackedSong;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZRepacker.Tests
{
    // Verifies object-encoded references are nullable and explicit JSON nulls survive
    [TestClass]
    public class TypeContractNullabilityTests
    {
        // Checks nullable metadata for every corrected object-encoded property
        [TestMethod]
        public void ObjectEncodedPropertiesAreNullable()
        {
            AssertNullable<Level>(nameof(Level.Name));
            AssertNullable<Level>(nameof(Level.TrileSetName));
            AssertNullable<Level>(nameof(Level.Volumes));
            AssertNullable<Level>(nameof(Level.Scripts));
            AssertNullable<Level>(nameof(Level.Triles));
            AssertNullable<Level>(nameof(Level.ArtObjects));
            AssertNullable<Level>(nameof(Level.BackgroundPlanes));
            AssertNullable<Level>(nameof(Level.Groups));
            AssertNullable<Level>(nameof(Level.NonPlayerCharacters));
            AssertNullable<Level>(nameof(Level.Paths));
            AssertNullable<Level>(nameof(Level.MutedLoops));
            AssertNullable<Level>(nameof(Level.AmbienceTracks));

            AssertNullable<NpcActionContent>(nameof(NpcActionContent.AnimationName));
            AssertNullable<NpcActionContent>(nameof(NpcActionContent.SoundName));
            AssertNullable<NpcInstance>(nameof(NpcInstance.Speech));
            AssertNullable<NpcInstance>(nameof(NpcInstance.Actions));
            AssertNullable<SpeechLine>(nameof(SpeechLine.Text));
            AssertNullable<AmbienceTrack>(nameof(AmbienceTrack.Name));
            AssertNullable<DotDialogueLine>(nameof(DotDialogueLine.ResourceText));

            AssertNullable<MovementPath>(nameof(MovementPath.Segments));
            AssertNullable<TrileGroup>(nameof(TrileGroup.Triles));
            AssertNullable<TrileInstance>(nameof(TrileInstance.OverlappedTriles));
            AssertNullable<Volume>(nameof(Volume.Orientations));
            AssertNullable<VolumeActorSettings>(nameof(VolumeActorSettings.DotDialogue));
            AssertNullable<ArtObjectActorSettings>(nameof(ArtObjectActorSettings.InvisibleSides));

            AssertNullable<Script>(nameof(Script.Triggers));
            AssertNullable<Script>(nameof(Script.Conditions));
            AssertNullable<Script>(nameof(Script.Actions));
            AssertNullable<ScriptAction>(nameof(ScriptAction.Object));
            AssertNullable<ScriptAction>(nameof(ScriptAction.Arguments));
            AssertNullable<ScriptTrigger>(nameof(ScriptTrigger.Object));
            AssertNullable<ScriptCondition>(nameof(ScriptCondition.Object));

            AssertNullable<TrackedSong>(nameof(TrackedSong.Loops));
            AssertNullable<TrackedSong>(nameof(TrackedSong.Notes));
            AssertNullable<Sky>(nameof(Sky.Layers));
            AssertNullable<Sky>(nameof(Sky.Clouds));
            AssertNullable<NpcMetadata>(nameof(NpcMetadata.SoundActions));

            AssertNullable<MapTree>(nameof(MapTree.Root));
            AssertNullable<MapNode>(nameof(MapNode.Connections));
            AssertNullable<MapNode>(nameof(MapNode.Conditions));
            AssertNullable<MapNodeConnection>(nameof(MapNodeConnection.Node));
            AssertNullable<WinConditions>(nameof(WinConditions.ScriptIds));

            AssertNullable<TrileSet>(nameof(TrileSet.Triles));
            AssertNullable<TrileSet>(nameof(TrileSet.TextureAtlas));
            AssertNullable<Trile>(nameof(Trile.Faces));
            AssertNullable<Trile>(nameof(Trile.Geometry));
            AssertNullable<ArtObject>(nameof(ArtObject.Cubemap));
            AssertNullable<ArtObject>(nameof(ArtObject.Geometry));
            AssertNullable<IndexedPrimitives<VertexInstance, Matrix>>(nameof(IndexedPrimitives<,>.Vertices));
            AssertNullable<IndexedPrimitives<VertexInstance, Matrix>>(nameof(IndexedPrimitives<,>.Indices));
        }

        // Deserializes and serializes representative nulls to verify JSON preservation
        [TestMethod]
        public void ExplicitJsonNullsArePreserved()
        {
            AssertJsonNullRoundTrip<NpcActionContent>(nameof(NpcActionContent.AnimationName));
            AssertJsonNullRoundTrip<NpcActionContent>(nameof(NpcActionContent.SoundName));
            AssertJsonNullRoundTrip<NpcInstance>(nameof(NpcInstance.Speech));
            AssertJsonNullRoundTrip<NpcInstance>(nameof(NpcInstance.Actions));
            AssertJsonNullRoundTrip<Script>(nameof(Script.Triggers));
            AssertJsonNullRoundTrip<Script>(nameof(Script.Conditions));
            AssertJsonNullRoundTrip<Script>(nameof(Script.Actions));
            AssertJsonNullRoundTrip<TrackedSong>(nameof(TrackedSong.Loops));
            AssertJsonNullRoundTrip<TrackedSong>(nameof(TrackedSong.Notes));
            AssertJsonNullRoundTrip<Sky>(nameof(Sky.Layers));
            AssertJsonNullRoundTrip<Sky>(nameof(Sky.Clouds));
            AssertJsonNullRoundTrip<ArtObjectInstance>(nameof(ArtObjectInstance.ActorSettings));
        }

        private static void AssertNullable<T>(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);

            Assert.IsNotNull(property, $"{typeof(T).Name}.{propertyName} does not exist.");

            var nullability = new NullabilityInfoContext().Create(property);

            Assert.AreEqual(
                NullabilityState.Nullable,
                nullability.WriteState,
                $"{typeof(T).Name}.{propertyName} is not declared nullable.");
        }

        private static void AssertJsonNullRoundTrip<T>(string propertyName)
        {
            var json = $"{{\"{propertyName}\":null}}";
            var value = JsonSerializer.Deserialize<T>(json);

            Assert.IsNotNull(value);

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(value));
            var property = document.RootElement.GetProperty(propertyName);

            Assert.AreEqual(JsonValueKind.Null, property.ValueKind);
        }
    }
}
