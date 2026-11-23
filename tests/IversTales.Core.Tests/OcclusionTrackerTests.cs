using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class OcclusionTrackerTests
    {
        [Fact]
        public void NewHits_StartOccluding()
        {
            var tracker = new OcclusionTracker<string>();
            tracker.Update(new[] { "wall", "pillar" });

            Assert.Equal(new[] { "wall", "pillar" }, tracker.StartedOccluding);
            Assert.Empty(tracker.StoppedOccluding);
        }

        [Fact]
        public void PersistentHits_AreNotReportedAgain()
        {
            // Regression: the fader started a new coroutine for every wall on every frame.
            var tracker = new OcclusionTracker<string>();
            tracker.Update(new[] { "wall" });
            tracker.Update(new[] { "wall" });
            tracker.Update(new[] { "wall" });

            Assert.Empty(tracker.StartedOccluding);
            Assert.Empty(tracker.StoppedOccluding);
            Assert.Single(tracker.Occluding);
        }

        [Fact]
        public void MissingHits_StopOccluding()
        {
            var tracker = new OcclusionTracker<string>();
            tracker.Update(new[] { "wall", "pillar" });
            tracker.Update(new[] { "pillar", "door" });

            Assert.Equal(new[] { "door" }, tracker.StartedOccluding);
            Assert.Equal(new[] { "wall" }, tracker.StoppedOccluding);
        }

        [Fact]
        public void NullsAndDuplicates_AreIgnored()
        {
            var tracker = new OcclusionTracker<string>();
            tracker.Update(new[] { "wall", null, "wall" });
            Assert.Single(tracker.StartedOccluding);

            tracker.Update(null);
            Assert.Equal(new[] { "wall" }, tracker.StoppedOccluding);
        }

        [Fact]
        public void Clear_ReportsEverythingAsStopped()
        {
            var tracker = new OcclusionTracker<string>();
            tracker.Update(new[] { "a", "b" });
            tracker.Clear();

            Assert.Equal(2, tracker.StoppedOccluding.Count);
            Assert.Empty(tracker.Occluding);
        }
    }
}
