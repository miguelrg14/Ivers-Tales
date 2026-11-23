using System.Collections;
using IversTales.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace IversTales.Tests
{
    /// <summary>
    /// End-to-end smoke test: loads every scene of the game and lets it run for
    /// two seconds. The Unity Test Framework fails the test on any error or
    /// exception logged meanwhile (plain Debug.Log messages are allowed).
    /// </summary>
    public class SceneSmokeTests
    {
        static readonly string[] Scenes = { GameScenes.MainMenu, GameScenes.Level1, GameScenes.Level2, GameScenes.Credits };

        [UnityTest]
        public IEnumerator Scene_LoadsAndRunsWithoutErrors([ValueSource(nameof(Scenes))] string scene)
        {
            LogAssert.ignoreFailingMessages = false; // errors and exceptions fail the test
            yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            Assert.AreEqual(scene, SceneManager.GetActiveScene().name);

            // Let Awake/Start run and a couple of seconds of gameplay (the intro screens pause the game).
            float end = Time.realtimeSinceStartup + 2f;
            while (Time.realtimeSinceStartup < end)
                yield return null;

            Assert.AreEqual(scene, SceneManager.GetActiveScene().name, "The scene was unloaded while running.");
        }

        [UnityTest]
        public IEnumerator Options_AreRestoredAfterReloadingTheMenu()
        {
            PlayerPrefs.SetFloat(VolumeSettings.KeyFor(VolumeChannel.Master), 0.5f);
            yield return SceneManager.LoadSceneAsync(GameScenes.MainMenu, LoadSceneMode.Single);
            yield return null;

            Assert.AreEqual(0.5f, AudioListener.volume, 0.001f);
            PlayerPrefs.DeleteKey(VolumeSettings.KeyFor(VolumeChannel.Master));
        }
    }
}
