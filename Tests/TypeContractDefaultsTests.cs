using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.Definitions.Game.MapTree;
using FEZRepacker.Core.Definitions.Game.NpcMetadata;
using FEZRepacker.Core.Definitions.Game.Sky;
using FEZRepacker.Core.Definitions.Game.TrackedSong;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.Definitions.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZRepacker.Tests
{
    // Verifies newly constructed repacker models use the same defaults as FEZ
    [TestClass]
    public class TypeContractDefaultsTests
    {
        // Constructs a level and checks the FEZ filtering, lighting, and collection defaults
        [TestMethod]
        public void LevelUsesFezDefaults()
        {
            var level = new Level();

            Assert.IsTrue(level.HaloFiltering);
            Assert.AreEqual(1f, level.BaseDiffuse);
            Assert.AreEqual(0.35f, level.BaseAmbient);
            Assert.AreEqual(0, level.Triles.Count);
            Assert.AreEqual(0, level.Volumes.Count);
            Assert.AreEqual(0, level.Scripts.Count);
            Assert.AreEqual(0, level.ArtObjects.Count);
            Assert.AreEqual(0, level.BackgroundPlanes.Count);
            Assert.AreEqual(0, level.Groups.Count);
            Assert.AreEqual(0, level.NonPlayerCharacters.Count);
            Assert.AreEqual(0, level.Paths.Count);
            Assert.AreEqual(0, level.MutedLoops.Count);
            Assert.AreEqual(0, level.AmbienceTracks.Count);
        }

        // Constructs level members and checks the collection defaults FEZ constructors provide
        [TestMethod]
        public void LevelMembersUseFezDefaults()
        {
            Assert.AreEqual(0, new TrileGroup().Triles.Count);
            Assert.AreEqual(0, new MovementPath().Segments.Count);
            Assert.AreEqual(0, new Volume().Orientations.Length);
            Assert.AreEqual(0, new VolumeActorSettings().DotDialogue.Count);
            Assert.AreEqual(0, new ArtObjectActorSettings().InvisibleSides.Length);
            Assert.AreEqual(0, new Script().Triggers.Count);
            Assert.AreEqual(0, new Script().Actions.Count);
            Assert.AreEqual("Untitled", new Script().Name);
            Assert.IsNull(new Script().Conditions);
            Assert.IsNull(new ScriptAction().Object);
            Assert.IsNull(new ScriptAction().Arguments);
            Assert.IsNull(new TrileInstance().ActorSettings);
        }

        // Constructs map models and checks which nodes FEZ leaves for the reader to supply
        [TestMethod]
        public void MapModelsUseFezDefaults()
        {
            var node = new MapNode();

            Assert.AreEqual(0, node.Connections.Count);
            Assert.IsNotNull(node.Conditions);
            Assert.AreEqual(0, node.Conditions.ScriptIds.Count);
            Assert.IsNull(new MapTree().Root);
            Assert.IsNull(new MapNodeConnection().Node);
        }

        // Constructs an art-object instance and checks its transform and settings defaults
        [TestMethod]
        public void ArtObjectInstanceUsesFezDefaults()
        {
            var instance = new ArtObjectInstance();

            Assert.AreEqual(Quaternion.Identity, instance.Rotation);
            Assert.AreEqual(Vector3.One, instance.Scale);
            Assert.IsNotNull(instance.ActorSettings);
        }

        // Constructs NPC models and checks their movement and collection defaults
        [TestMethod]
        public void NpcModelsUseFezDefaults()
        {
            var instance = new NpcInstance();
            var metadata = new NpcMetadata();

            Assert.AreEqual(1.5f, instance.WalkSpeed);
            Assert.IsNotNull(instance.Speech);
            Assert.AreEqual(0, instance.Speech.Count);
            Assert.IsNotNull(instance.Actions);
            Assert.AreEqual(0, instance.Actions.Count);
            Assert.AreEqual(1.5f, metadata.WalkSpeed);
            Assert.IsNotNull(metadata.SoundActions);
            Assert.AreEqual(0, metadata.SoundActions.Count);
        }

        // Constructs song models and checks FEZ timing, note, and loop defaults
        [TestMethod]
        public void TrackedSongModelsUseFezDefaults()
        {
            var song = new TrackedSong();
            var loop = new Loop();
            var expectedNotes = new[]
            {
                ShardNotes.C2, ShardNotes.D2, ShardNotes.E2, ShardNotes.F2, ShardNotes.G2, ShardNotes.A2,
                ShardNotes.B2, ShardNotes.C3
            };

            Assert.AreEqual("Untitled", song.Name);
            Assert.AreEqual(60, song.Tempo);
            Assert.AreEqual(4, song.TimeSignature);
            CollectionAssert.AreEqual(expectedNotes, song.Notes);
            Assert.IsNotNull(song.Loops);
            Assert.AreEqual(0, song.Loops.Count);
            Assert.AreEqual(1, loop.Duration);
            Assert.AreEqual(1, loop.LoopTimesFrom);
            Assert.AreEqual(1, loop.LoopTimesTo);
            Assert.IsTrue(loop.Day);
            Assert.IsTrue(loop.Night);
            Assert.IsTrue(loop.Dawn);
            Assert.IsTrue(loop.Dusk);
        }

        // Constructs sky models and checks their rendering defaults
        [TestMethod]
        public void SkyModelsUseFezDefaults()
        {
            var sky = new Sky();
            var layer = new SkyLayer();

            Assert.AreEqual("Default", sky.Name);
            Assert.AreEqual("SkyBack", sky.Background);
            Assert.AreEqual(1f, sky.WindSpeed);
            Assert.AreEqual(1f, sky.Density);
            Assert.AreEqual(0.02f, sky.FogDensity);
            Assert.AreEqual(0.5f, sky.LayerBaseHeight);
            Assert.AreEqual(1f, sky.CloudsParallax);
            Assert.AreEqual(0.7f, sky.ShadowOpacity);
            Assert.IsNotNull(sky.Layers);
            Assert.IsNotNull(sky.Clouds);
            Assert.AreEqual(1f, layer.Opacity);
        }

        // Constructs a trile and checks its resource, size, and face defaults
        [TestMethod]
        public void TrileUsesFezDefaults()
        {
            var trile = new Trile();

            Assert.AreEqual("Untitled", trile.Name);
            Assert.AreEqual(Vector3.One, trile.Size);
            Assert.IsNotNull(trile.Faces);
            Assert.AreEqual(0, trile.Faces.Count);
            Assert.IsNull(trile.Geometry);
        }

        // Deconverts a map model and verifies the converter creates its required root
        [TestMethod]
        public void MapTreeJsonModelCreatesRoot()
        {
            var model = new MapTreeJsonModel { [0] = new MapNodeJsonModel() };
            var mapTree = model.Deserialize();

            Assert.IsNotNull(mapTree.Root);
        }
    }
}