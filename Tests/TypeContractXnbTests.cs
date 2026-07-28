using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
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
                Volumes = null,
                Scripts = null,
                Triles = null,
                ArtObjects = null,
                BackgroundPlanes = null,
                Groups = null,
                NonPlayerCharacters = null,
                Paths = null,
                MutedLoops = null,
                AmbienceTracks = null
            });
            var song = RoundTrip(new TrackedSong
            {
                Loops = null,
                Notes = null
            });
            var sky = RoundTrip(new Sky
            {
                Layers = null,
                Clouds = null
            });
            var metadata = RoundTrip(new NpcMetadata
            {
                SoundActions = null
            });
            var mapTree = RoundTrip(new MapTree
            {
                Root = null
            });
            var trileSet = RoundTrip(new TrileSet
            {
                Triles = null,
                TextureAtlas = null
            });
            var artObject = RoundTrip(new ArtObject
            {
                Name = "TEST_ART_OBJECT",
                Cubemap = null,
                Geometry = null
            });

            Assert.IsNull(level.Volumes);
            Assert.IsNull(level.Scripts);
            Assert.IsNull(level.Triles);
            Assert.IsNull(level.ArtObjects);
            Assert.IsNull(level.BackgroundPlanes);
            Assert.IsNull(level.Groups);
            Assert.IsNull(level.NonPlayerCharacters);
            Assert.IsNull(level.Paths);
            Assert.IsNull(level.MutedLoops);
            Assert.IsNull(level.AmbienceTracks);
            Assert.IsNull(song.Loops);
            Assert.IsNull(song.Notes);
            Assert.IsNull(sky.Layers);
            Assert.IsNull(sky.Clouds);
            Assert.IsNull(metadata.SoundActions);
            Assert.IsNull(mapTree.Root);
            Assert.IsNull(trileSet.Triles);
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
