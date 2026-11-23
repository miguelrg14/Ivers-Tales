using System;
using System.Collections.Generic;
using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class SoundRegistryTests
    {
        static KeyValuePair<string, object> Entry(string key, object clip) => new KeyValuePair<string, object>(key, clip);

        [Fact]
        public void TryGet_ReturnsRegisteredClip()
        {
            var clip = new object();
            var registry = new SoundRegistry<string, object>(new[] { Entry("dash", clip) });

            Assert.True(registry.TryGet("dash", out var found));
            Assert.Same(clip, found);
            Assert.False(registry.TryGet("jump", out _));
        }

        [Fact]
        public void DuplicatedKey_KeepsFirstClipAndDoesNotThrow()
        {
            // Regression: the original code checked ContainsValue(clip) and then Add(key),
            // which throws ArgumentException when two entries share a key.
            var first = new object();
            var registry = new SoundRegistry<string, object>(new[] { Entry("hurt", first), Entry("hurt", new object()) });

            Assert.True(registry.TryGet("hurt", out var found));
            Assert.Same(first, found);
            Assert.Equal(new[] { "hurt" }, registry.DuplicatedKeys);
        }

        [Fact]
        public void SameClipUnderTwoKeys_IsAllowed()
        {
            var clip = new object();
            var registry = new SoundRegistry<string, object>(new[] { Entry("a", clip), Entry("b", clip) });
            Assert.Equal(2, registry.Count);
        }

        [Fact]
        public void NullClips_AreSkipped()
        {
            var registry = new SoundRegistry<string, object>(new[] { Entry("a", null) });
            Assert.Equal(0, registry.Count);
        }

        [Fact]
        public void Constructor_RejectsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SoundRegistry<string, object>(null));
        }
    }
}
