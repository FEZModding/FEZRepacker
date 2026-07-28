using System.Reflection;
using System.Text.Json;

using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.Definitions.Game.TrackedSong;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZRepacker.Tests
{
    // Verifies nullable aliases that are not represented by FEZ type metadata
    [TestClass]
    public class TypeContractSemanticNullabilityTests
    {
        // Checks repacker-only entity aliases retain their nullable write contract
        [TestMethod]
        public void EntityAliasesAreNullable()
        {
            AssertNullable<ScriptAction>(nameof(ScriptAction.Object));
            AssertNullable<ScriptTrigger>(nameof(ScriptTrigger.Object));
            AssertNullable<ScriptCondition>(nameof(ScriptCondition.Object));
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
