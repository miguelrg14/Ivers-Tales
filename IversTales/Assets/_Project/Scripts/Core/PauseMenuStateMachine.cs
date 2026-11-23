using System;

namespace IversTales.Core
{
    public enum PauseMenuState
    {
        /// <summary>Gameplay running, HUD visible.</summary>
        Playing,
        /// <summary>Pause menu open.</summary>
        Paused,
        /// <summary>Options opened from the pause menu.</summary>
        Options,
        /// <summary>"This is a demo" disclaimer shown at the start of level 1.</summary>
        Disclaimer,
        /// <summary>Controls / welcome screen shown after the disclaimer.</summary>
        Welcome,
        /// <summary>"You gained a new spell" screen shown at the start of level 2.</summary>
        NewSpell
    }

    /// <summary>
    /// State machine behind the in-game UI (HUD, pause, options and the intro screens).
    /// It decides which panel is visible and whether the game is paused, so
    /// <c>UIManager</c> only has to mirror the state into the scene.
    /// </summary>
    public sealed class PauseMenuStateMachine
    {
        public PauseMenuState State { get; private set; } = PauseMenuState.Playing;

        /// <summary>Raised with (previous, current) every time the state changes.</summary>
        public event Action<PauseMenuState, PauseMenuState> StateChanged;

        /// <summary>True for every state except <see cref="PauseMenuState.Playing"/>.</summary>
        public bool IsPaused => State != PauseMenuState.Playing;

        /// <summary>Time.timeScale that matches the current state.</summary>
        public float TimeScale => IsPaused ? 0f : 1f;

        public bool IsShowingIntro =>
            State == PauseMenuState.Disclaimer ||
            State == PauseMenuState.Welcome ||
            State == PauseMenuState.NewSpell;

        /// <summary>Shows the intro screen for a level (disclaimer for level 1, new spell for level 2).</summary>
        public void ShowIntro(bool isNewSpellLevel)
        {
            SetState(isNewSpellLevel ? PauseMenuState.NewSpell : PauseMenuState.Disclaimer);
        }

        /// <summary>Confirm button on an intro screen: disclaimer → welcome → playing.</summary>
        public void ConfirmIntro()
        {
            switch (State)
            {
                case PauseMenuState.Disclaimer:
                    SetState(PauseMenuState.Welcome);
                    break;
                case PauseMenuState.Welcome:
                case PauseMenuState.NewSpell:
                    SetState(PauseMenuState.Playing);
                    break;
            }
        }

        /// <summary>Menu key: opens the pause menu, or goes back one step. Ignored on intro screens.</summary>
        public void TogglePause()
        {
            switch (State)
            {
                case PauseMenuState.Playing:
                    SetState(PauseMenuState.Paused);
                    break;
                case PauseMenuState.Paused:
                case PauseMenuState.Options:
                    SetState(PauseMenuState.Playing);
                    break;
            }
        }

        /// <summary>Opens the pause menu from gameplay, or goes back to it from the options.</summary>
        public void ShowPauseMenu()
        {
            if (State == PauseMenuState.Playing || State == PauseMenuState.Options)
                SetState(PauseMenuState.Paused);
        }

        public void OpenOptions()
        {
            if (State == PauseMenuState.Paused)
                SetState(PauseMenuState.Options);
        }

        public void Resume()
        {
            if (!IsShowingIntro)
                SetState(PauseMenuState.Playing);
        }

        /// <summary>Skips every intro/pause screen and returns to gameplay.</summary>
        public void ForcePlaying() => SetState(PauseMenuState.Playing);

        void SetState(PauseMenuState next)
        {
            if (next == State) return;
            var previous = State;
            State = next;
            StateChanged?.Invoke(previous, next);
        }
    }
}
