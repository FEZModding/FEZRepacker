using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.Definitions.Game.TrackedSong;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.XNB;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZRepacker.Tests
{
    // Verifies representative nullable values survive the repacker XNB pipeline
    [TestClass]
    public class TypeContractXnbTests
    {
        // Serializes required strings, models, and arrays set to null and checks the contract error
        [TestMethod]
        public void RequiredReferencePropertyCannotBeSerializedAsNull()
        {
            AssertRequiredPropertyValidation(
                new Level { SkyName = null! },
                nameof(Level),
                nameof(Level.SkyName));

            AssertRequiredPropertyValidation(
                new SpriteFont { Texture = null! },
                nameof(SpriteFont),
                nameof(SpriteFont.Texture));

            AssertRequiredPropertyValidation(
                new Texture2D { TextureData = null! },
                nameof(Texture2D),
                nameof(Texture2D.TextureData));

            AssertRequiredPropertyValidation(
                new Level { SkyName = "TEST_SKY", Triles = null! },
                nameof(Level),
                nameof(Level.Triles));

            AssertRequiredPropertyValidation(
                new TrackedSong { Notes = null! },
                nameof(TrackedSong),
                nameof(TrackedSong.Notes));

            AssertRequiredPropertyValidation(
                new Sky { Clouds = null! },
                nameof(Sky),
                nameof(Sky.Clouds));

            AssertRequiredPropertyValidation(
                new MapTree { Root = new MapNode { Conditions = null! } },
                nameof(MapNode),
                nameof(MapNode.Conditions));
        }

        private static void AssertRequiredPropertyValidation(object model, string modelName, string propertyName)
        {
            var exception = Assert.ThrowsException<InvalidOperationException>(() => XnbSerializer.Serialize(model));
            StringAssert.Contains(exception.Message, $"{modelName}.{propertyName}");
        }

        // Packs and reads an NPC action with no sound to prevent empty-string regression
        [TestMethod]
        public void NpcActionNullSoundSurvivesXnbRoundTrip()
        {
            var level = new Level
            {
                Name = "TEST_LEVEL",
                SkyName = "TEST_SKY",
                TrileSetName = "TEST_TRILE_SET",
                NonPlayerCharacters = new Dictionary<int, NpcInstance>
                {
                    [0] = new()
                    {
                        Name = "TEST_NPC",
                        Actions = new Dictionary<NpcAction, NpcActionContent>
                        {
                            [NpcAction.Idle] = new()
                            {
                                AnimationName = "Idle",
                                SoundName = null
                            }
                        }
                    }
                }
            };

            using var stream = XnbSerializer.Serialize(level);
            var result = XnbSerializer.Deserialize(stream) as Level;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.NonPlayerCharacters);

            var npc = result.NonPlayerCharacters[0];
            Assert.IsNotNull(npc.Actions);
            Assert.IsNull(npc.Actions[NpcAction.Idle].SoundName);
        }

        // Packs primary asset types with null nested objects and verifies their markers survive
        [TestMethod]
        public void ObjectEncodedNullsSurviveXnbRoundTrip()
        {
            var level = RoundTrip(new Level
            {
                SkyName = "TEST_SKY",
                Name = null,
                TrileSetName = null,
                SongName = null,
                StartingFace = null,
                Scripts = new Dictionary<int, Script>
                {
                    [0] = new() { Conditions = null, Actions = { new ScriptAction { Object = null } } }
                }
            });
            var song = RoundTrip(new TrackedSong
            {
                CustomOrdering = null
            });
            var sky = RoundTrip(new Sky
            {
                Shadows = null,
                Stars = null,
                CloudTint = null
            });
            var metadata = RoundTrip(new NpcMetadata
            {
                SoundPath = null
            });
            var mapTree = RoundTrip(new MapTree
            {
                Root = null
            });
            var trileSet = RoundTrip(new TrileSet
            {
                Triles = new Dictionary<int, Trile> { [0] = new() { Geometry = null } },
                TextureAtlas = null
            });
            var artObject = RoundTrip(new ArtObject
            {
                Name = "TEST_ART_OBJECT",
                Cubemap = null,
                Geometry = null
            });

            Assert.IsNull(level.Name);
            Assert.IsNull(level.TrileSetName);
            Assert.IsNull(level.SongName);
            Assert.IsNull(level.StartingFace);
            Assert.IsNull(level.Scripts[0].Conditions);
            Assert.IsNull(level.Scripts[0].Actions[0].Object);
            Assert.IsNull(level.Scripts[0].Actions[0].Arguments);
            Assert.IsNull(song.CustomOrdering);
            Assert.IsNull(sky.Shadows);
            Assert.IsNull(sky.Stars);
            Assert.IsNull(sky.CloudTint);
            Assert.IsNull(metadata.SoundPath);
            Assert.IsNull(mapTree.Root);
            Assert.IsNull(trileSet.Triles[0].Geometry);
            Assert.IsNull(trileSet.TextureAtlas);
            Assert.IsNull(artObject.Cubemap);
            Assert.IsNull(artObject.Geometry);
        }

        private static T RoundTrip<T>(T value) where T : class
        {
            using var stream = XnbSerializer.Serialize(value);
            var result = XnbSerializer.Deserialize(stream) as T;
            Assert.IsNotNull(result);
            return result;
        }
    }
}
