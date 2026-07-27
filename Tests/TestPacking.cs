using Microsoft.VisualStudio.TestTools.UnitTesting;
using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Tests
{
    // Verifies package data is byte-identical after unpacking and repacking
    [TestClass]
    public class TestPacking
    {
        // Rebuilds each configured FEZ package and compares its complete payload
        [TestMethod]
        [DynamicData(nameof(TestUtils.PackagePathsTestData), typeof(TestUtils))]
        public void RepackAndComparePackage(string? packagePath)
        {
            if (packagePath == null)
            {
                Assert.Inconclusive("Set FEZContentDirPath in a runsettings file to run package integration tests.");
                return;
            }

            var pakData = File.ReadAllBytes(packagePath);

            using var pakStream = new MemoryStream(pakData);
            using var pakReader = new PakReader(pakStream);

            using var repackStream = new MemoryStream();
            using var repackWriter = new PakWriter(repackStream);

            foreach (var item in pakReader.ReadFiles())
            {
                repackWriter.WriteFile(item.Path, new MemoryStream(item.Payload));
            }

            repackWriter.Dispose();
            var repackData = repackStream.ToArray();

            Assert.IsTrue(repackData.SequenceEqual(pakData));
        }
    }
}
