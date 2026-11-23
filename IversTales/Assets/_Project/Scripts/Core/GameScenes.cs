namespace IversTales.Core
{
    /// <summary>Scene names as listed in the build settings.</summary>
    public static class GameScenes
    {
        public const string MainMenu = "MainMenu";
        public const string Level1 = "Level1";
        public const string Level2 = "Level2";
        public const string Credits = "Credits";

        /// <summary>Scene loaded when the player reaches the exit of <paramref name="scene"/>.</summary>
        public static string NextAfter(string scene)
        {
            switch (scene)
            {
                case Level1: return Level2;
                case Level2: return Credits;
                case Credits: return MainMenu;
                default: return Level1;
            }
        }

        /// <summary>Level 2 opens with the "new spell" screen instead of the disclaimer.</summary>
        public static bool StartsWithNewSpellScreen(string scene) => scene == Level2;
    }
}
