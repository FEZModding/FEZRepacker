using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZRepacker.Tests
{
    [TestClass]
    public class TestUtils
    {
        public static TestContext Context;

        [AssemblyInitialize]
        public static void SetupTestContext(TestContext testContext)
        {
            Context = testContext;
            Assert.IsTrue(
                Directory.Exists(GetGameAssetsDirectory()), 
                "You forgot to put FEZ's Content path into .runsettings file"
            );
        }

        public static IEnumerable<object[]> PackagePathsTestData => 
            GetPathsToPackages().Select(name => new object[] { name });

        public static string GetGameAssetsDirectory()
        {
            return Context.Properties["FEZContentDirPath"].ToString();
        }

        public static IEnumerable<string> GetPathsToPackages()
        {
            return Directory.EnumerateFiles(GetGameAssetsDirectory(), "*.pak", SearchOption.AllDirectories);
        }
    }
}
