using Microsoft.VisualStudio.TestTools.UnitTesting;

using FEZRepacker.Core.Conversion;
using FEZRepacker.Core.Definitions.Game.ArtObject;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.TrileSet;
using FEZRepacker.Core.FileSystem;
using FEZRepacker.Core.XNB;

namespace FEZRepacker.Tests
{
    // Verifies assets survive being converted into editable formats and back
    [TestClass]
    public class RoundTripConversion
    {
        private const int ReportedExampleLimit = 10;

        // Converts every asset of a package through each format it can be edited in
        [TestMethod]
        [DynamicData(nameof(TestUtils.PackagePathsTestData), typeof(TestUtils))]
        public void ConvertAndCompareAssets(string? packagePath)
        {
            if (packagePath == null)
            {
                Assert.Inconclusive("Set FEZContentDirPath in a runsettings file to run package integration tests.");
                return;
            }

            var identical = 0;
            var equivalent = new List<string>();
            var different = new List<string>();

            using var packageStream = File.OpenRead(packagePath);
            using var packageReader = new PakReader(packageStream);

            foreach (var item in packageReader.ReadFiles())
            {
                if (FindAssetType(item.Payload) is not { } assetType) continue;

                foreach (var (format, settings) in EditableFormatsOf(assetType))
                {
                    var label = $"{item.Path} [{assetType.Name} as {format}]";

                    // Comparing consumes the asset it is given, so every format starts anew.
                    var asset = TryDeserialize(item.Payload);
                    if (asset == null)
                    {
                        different.Add($"{label}: asset could not be read");
                        continue;
                    }

                    switch (RoundTripAndCompare(asset, settings, out var difference))
                    {
                        case AssetsComparison.Result.Identical:
                            identical++;
                            break;
                        case AssetsComparison.Result.Equivalent:
                            equivalent.Add(label);
                            break;
                        default:
                            different.Add($"{label}: {difference}");
                            break;
                    }
                }
            }

            var package = Path.GetFileName(packagePath);

            Assert.IsTrue(different.Count == 0,
                $"{package}: {different.Count} assets lost data when converted."
                + $"{Environment.NewLine}{Summarize(different)}");

            if (equivalent.Count > 0)
            {
                Assert.Inconclusive(
                    $"{package}: {identical} assets converted exactly, {equivalent.Count} only as far as"
                    + $" their format allows.{Environment.NewLine}{Summarize(equivalent)}");
            }
        }

        private static AssetsComparison.Result RoundTripAndCompare(
            object asset,
            FormatConverterSettings settings,
            out string difference)
        {
            try
            {
                using var bundle = FormatConversion.Convert(asset, settings);
                var roundTripped = FormatConversion.Deconvert(bundle, settings);
                if (roundTripped == null)
                {
                    difference = "deconverting the bundle gave nothing back";
                    return AssetsComparison.Result.Different;
                }

                return AssetsComparison.ConsumeAndCompare(asset, roundTripped, out difference);
            }
            catch (Exception exception)
            {
                difference = $"{exception.GetType().Name}: {exception.Message}";
                return AssetsComparison.Result.Different;
            }
        }

        private static IEnumerable<(string Format, FormatConverterSettings Settings)> EditableFormatsOf(Type assetType)
        {
            if (assetType == typeof(ArtObject) || assetType == typeof(TrileSet))
            {
                yield return ("glTF", new FormatConverterSettings());
                yield return ("trixel art bundle", new FormatConverterSettings { UseTrixelArtBundle = true });
            }
            else if (assetType == typeof(AnimatedTexture))
            {
                yield return ("GIF", new FormatConverterSettings());
                yield return ("animation sheet", new FormatConverterSettings { UseAnimationSheet = true });
            }
            else
            {
                yield return ("its own format", new FormatConverterSettings());
            }
        }

        // Packages also hold files which aren't XNB assets at all
        private static Type? FindAssetType(byte[] payload)
        {
            try
            {
                using var stream = new MemoryStream(payload);
                return XnbSerializer.DeserializePrimaryContentTypeOnly(stream);
            }
            catch
            {
                return null;
            }
        }

        private static object? TryDeserialize(byte[] payload)
        {
            try
            {
                using var stream = new MemoryStream(payload);
                return XnbSerializer.Deserialize(stream);
            }
            catch
            {
                return null;
            }
        }

        private static string Summarize(List<string> entries)
        {
            var listed = string.Join(Environment.NewLine, entries.Take(ReportedExampleLimit));
            if (entries.Count <= ReportedExampleLimit) return listed;

            return $"{listed}{Environment.NewLine}...and {entries.Count - ReportedExampleLimit} more.";
        }
    }
}
