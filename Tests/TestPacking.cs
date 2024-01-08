using Microsoft.VisualStudio.TestTools.UnitTesting;
using FEZRepacker.Converter.FileSystem;

namespace FEZRepacker.Tests
{
    [TestClass]
    public class TestPacking
    {
        [TestMethod]
        [DynamicData(nameof(TestUtils.PackagePathsTestData), typeof(TestUtils))]
        public void RepackAndComparePackage(string packagePath)
        {
            var pakData = File.ReadAllBytes(packagePath);

            using var pakStream = new MemoryStream(pakData);
            using var pakReader = new PakReader(pakStream);

            using var repackStream = new MemoryStream();
            using var repackWriter = new PakWriter(repackStream);

            foreach (var item in pakReader.ReadFiles())
            {
                repackWriter.WriteFile(item.Path, item.Data);
            }

            repackWriter.Dispose();
            var repackData = repackStream.ToArray();

            Assert.IsTrue(repackData.SequenceEqual(pakData));
        }
    }
}