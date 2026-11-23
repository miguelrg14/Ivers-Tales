using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class GameScenesTests
    {
        [Theory]
        [InlineData(GameScenes.MainMenu, GameScenes.Level1)]
        [InlineData(GameScenes.Level1, GameScenes.Level2)]
        [InlineData(GameScenes.Level2, GameScenes.Credits)]
        [InlineData(GameScenes.Credits, GameScenes.MainMenu)]
        public void NextAfter_FollowsTheGameFlow(string scene, string expected)
        {
            Assert.Equal(expected, GameScenes.NextAfter(scene));
        }

        [Fact]
        public void OnlyLevel2_StartsWithNewSpellScreen()
        {
            Assert.True(GameScenes.StartsWithNewSpellScreen(GameScenes.Level2));
            Assert.False(GameScenes.StartsWithNewSpellScreen(GameScenes.Level1));
        }
    }
}
