using System.Collections.Generic;
using IversTales.Core;
using Xunit;

namespace IversTales.Core.Tests
{
    public class PauseMenuStateMachineTests
    {
        [Fact]
        public void StartsPlaying()
        {
            var menu = new PauseMenuStateMachine();
            Assert.Equal(PauseMenuState.Playing, menu.State);
            Assert.False(menu.IsPaused);
            Assert.Equal(1f, menu.TimeScale);
        }

        [Fact]
        public void Level1Intro_GoesDisclaimerThenWelcomeThenPlaying()
        {
            var menu = new PauseMenuStateMachine();
            menu.ShowIntro(isNewSpellLevel: false);
            Assert.Equal(PauseMenuState.Disclaimer, menu.State);
            Assert.Equal(0f, menu.TimeScale);

            menu.ConfirmIntro();
            Assert.Equal(PauseMenuState.Welcome, menu.State);

            menu.ConfirmIntro();
            Assert.Equal(PauseMenuState.Playing, menu.State);
            Assert.Equal(1f, menu.TimeScale);
        }

        [Fact]
        public void Level2Intro_ShowsNewSpellScreen()
        {
            var menu = new PauseMenuStateMachine();
            menu.ShowIntro(isNewSpellLevel: true);
            Assert.Equal(PauseMenuState.NewSpell, menu.State);

            menu.ConfirmIntro();
            Assert.Equal(PauseMenuState.Playing, menu.State);
        }

        [Fact]
        public void TogglePause_IsIgnoredDuringIntro()
        {
            var menu = new PauseMenuStateMachine();
            menu.ShowIntro(false);

            menu.TogglePause();
            menu.Resume();

            Assert.Equal(PauseMenuState.Disclaimer, menu.State);
        }

        [Fact]
        public void TogglePause_OpensAndClosesPauseMenu()
        {
            var menu = new PauseMenuStateMachine();
            menu.TogglePause();
            Assert.Equal(PauseMenuState.Paused, menu.State);
            menu.TogglePause();
            Assert.Equal(PauseMenuState.Playing, menu.State);
        }

        [Fact]
        public void Options_OnlyFromPauseMenu_AndMenuKeyResumes()
        {
            var menu = new PauseMenuStateMachine();
            menu.OpenOptions();
            Assert.Equal(PauseMenuState.Playing, menu.State);

            menu.TogglePause();
            menu.OpenOptions();
            Assert.Equal(PauseMenuState.Options, menu.State);
            Assert.True(menu.IsPaused);

            menu.TogglePause();
            Assert.Equal(PauseMenuState.Playing, menu.State);
        }

        [Fact]
        public void ShowPauseMenu_ReturnsFromOptions_AndIsIgnoredDuringIntro()
        {
            var menu = new PauseMenuStateMachine();
            menu.ShowPauseMenu();
            menu.OpenOptions();
            menu.ShowPauseMenu();
            Assert.Equal(PauseMenuState.Paused, menu.State);

            var intro = new PauseMenuStateMachine();
            intro.ShowIntro(true);
            intro.ShowPauseMenu();
            Assert.Equal(PauseMenuState.NewSpell, intro.State);
        }

        [Fact]
        public void StateChanged_ReportsTransitionsOnce()
        {
            var menu = new PauseMenuStateMachine();
            var log = new List<(PauseMenuState, PauseMenuState)>();
            menu.StateChanged += (from, to) => log.Add((from, to));

            menu.TogglePause();
            menu.OpenOptions();
            menu.OpenOptions();
            menu.Resume();

            Assert.Equal(new[]
            {
                (PauseMenuState.Playing, PauseMenuState.Paused),
                (PauseMenuState.Paused, PauseMenuState.Options),
                (PauseMenuState.Options, PauseMenuState.Playing),
            }, log);
        }

        [Fact]
        public void ForcePlaying_SkipsIntro()
        {
            var menu = new PauseMenuStateMachine();
            menu.ShowIntro(false);
            menu.ForcePlaying();
            Assert.Equal(PauseMenuState.Playing, menu.State);
        }
    }
}
