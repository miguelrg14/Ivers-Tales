using System.Collections.Generic;

namespace IversTales.Core
{
    /// <summary>
    /// Keeps track of the objects standing between the camera and the player.
    /// Each frame the camera reports what its ray hit; the tracker answers which
    /// objects just started blocking the view (fade them out) and which ones stopped
    /// blocking it (fade them back in). Objects that keep blocking are not reported
    /// again, so each wall is faded once instead of once per frame.
    /// </summary>
    public sealed class OcclusionTracker<T>
    {
        readonly HashSet<T> occluding = new HashSet<T>();
        readonly List<T> started = new List<T>();
        readonly List<T> stopped = new List<T>();

        public IReadOnlyCollection<T> Occluding => occluding;
        public IReadOnlyList<T> StartedOccluding => started;
        public IReadOnlyList<T> StoppedOccluding => stopped;

        public void Update(IEnumerable<T> currentlyHit)
        {
            started.Clear();
            stopped.Clear();
            var current = new HashSet<T>();
            if (currentlyHit != null)
                foreach (var item in currentlyHit)
                    if (item != null) current.Add(item);

            foreach (var item in current)
                if (!occluding.Contains(item)) started.Add(item);
            foreach (var item in occluding)
                if (!current.Contains(item)) stopped.Add(item);

            occluding.Clear();
            occluding.UnionWith(current);
        }

        /// <summary>Forgets everything and reports every tracked object as no longer occluding.</summary>
        public void Clear()
        {
            started.Clear();
            stopped.Clear();
            stopped.AddRange(occluding);
            occluding.Clear();
        }
    }
}
