using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZRepacker.Tests
{
    // Supplies optional FEZ asset paths to package integration tests
    [TestClass]
    public class TestUtils
    {
        private static TestContext Context { get; set; } = null!;

        // Captures the MSTest context without requiring optional integration-test data
        [AssemblyInitialize]
        public static void SetupTestContext(TestContext testContext)
        {
            Context = testContext;
        }

        public static IEnumerable<object?[]> PackagePathsTestData
        {
            get
            {
                var assetsDirectory = GetGameAssetsDirectory();
                if (assetsDirectory == null)
                {
                    yield return new object?[] { null };
                    yield break;
                }

                foreach (var packagePath in GetPathsToPackages(assetsDirectory))
                {
                    yield return new object?[] { packagePath };
                }
            }
        }

        private static string? GetGameAssetsDirectory()
        {
            var configuredPath = Context.Properties["FEZContentDirPath"]?.ToString();
            return string.IsNullOrWhiteSpace(configuredPath) ? null :
                Directory.Exists(configuredPath) ? configuredPath : null;
        }

        private static IEnumerable<string> GetPathsToPackages(string assetsDirectory)
        {
            return Directory.EnumerateFiles(assetsDirectory, "*.pak", SearchOption.AllDirectories);
        }
    }
}
