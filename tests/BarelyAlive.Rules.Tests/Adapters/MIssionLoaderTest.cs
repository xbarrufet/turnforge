using System;
using System.IO;
using NUnit.Framework;
using BarelyAlive.Rules.Adapter.Loaders;
using System.Linq;

namespace BarelyAlive.Rules.Test.Adapters
{
    [TestFixture]
    public class MissionLoaderTest
    {
        [Test]
        public void ParseMissionString_WithAllMission01Json_ReturnsDescriptors()
        {
            // try multiple candidate filenames (correct and common misspelling)
            var candidateFiles = new[] { "all_mission01.json", "all_mision01.json" };

            var assetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");
            var candidates = candidateFiles
                .SelectMany(n => new[] { Path.Combine(assetsDir, n), Path.Combine(AppContext.BaseDirectory, n) })
                .ToList();

            // also try current directory fallback
            candidates.AddRange(candidateFiles.SelectMany(n => new[] { Path.Combine(Directory.GetCurrentDirectory(), "Assets", n), Path.Combine(Directory.GetCurrentDirectory(), n) }));

            string? path = candidates.FirstOrDefault(File.Exists);

            if (path == null)
            {
                string dirList = string.Join("\n", new[] { assetsDir, AppContext.BaseDirectory, Path.Combine(Directory.GetCurrentDirectory(), "Assets"), Directory.GetCurrentDirectory() }
                    .Select(bp => bp + ":\n  " + (Directory.Exists(bp) ? string.Join("\n  ", Directory.GetFiles(bp).Select(Path.GetFileName)) : "(not found)")));
                Assert.Fail("Could not find any of the expected test files. Tried:\n" + string.Join("\n", candidates) + "\n\nDirectory listings:\n" + dirList);
            }

            // sanity
            Assert.That(File.Exists(path), Is.True, $"Test data not found: {path}");

            var json = File.ReadAllText(path);

            var (spatial, zones, props) = MissionLoader.ParseMissionString(json);

            Assert.That(spatial, Is.Not.Null, "Spatial descriptor should not be null");
            Assert.That(zones, Is.Not.Null, "Zones descriptor should not be null");
            Assert.That(props, Is.Not.Null, "Props descriptor should not be null");
        }
    }
}