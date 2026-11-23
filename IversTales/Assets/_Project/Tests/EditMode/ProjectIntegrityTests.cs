using System.IO;
using System.Linq;
using IversTales.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IversTales.Tests
{
    /// <summary>
    /// Checks that the project is in a shippable state: the build contains the
    /// expected scenes and no scene or prefab has broken script references.
    /// </summary>
    public class ProjectIntegrityTests
    {
        static readonly string[] ExpectedScenes = { GameScenes.MainMenu, GameScenes.Level1, GameScenes.Level2, GameScenes.Credits };

        [Test]
        public void BuildSettings_ContainTheGameScenesInOrder()
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => Path.GetFileNameWithoutExtension(s.path)).ToArray();
            CollectionAssert.AreEqual(ExpectedScenes, scenes);
        }

        [Test]
        public void BuildSettings_ScenesExistOnDisk()
        {
            foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
                Assert.IsTrue(File.Exists(scene.path), $"Missing scene file: {scene.path}");
        }

        [TestCaseSource(nameof(ExpectedScenes))]
        public void Scene_HasNoMissingScripts(string sceneName)
        {
            var path = EditorBuildSettings.scenes.First(s => Path.GetFileNameWithoutExtension(s.path) == sceneName).path;
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            int missing = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));

            Assert.AreEqual(0, missing, $"{sceneName} has {missing} missing script(s).");
        }

        [Test]
        public void Prefabs_HaveNoMissingScripts()
        {
            var broken = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path =>
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    return prefab.GetComponentsInChildren<Transform>(true)
                        .Any(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0);
                })
                .ToList();

            Assert.IsEmpty(broken, "Prefabs with missing scripts:\n" + string.Join("\n", broken));
        }

        [TestCase("Music", "musicVolume")]
        [TestCase("SFX", "sfxVolume")]
        [TestCase("Interface", "uiVolume")]
        [TestCase("Dialogues", "voiceVolume")]
        public void AudioMixer_ExposesItsVolumeParameter(string mixerName, string parameter)
        {
            var mixer = AssetDatabase.FindAssets("t:AudioMixer", new[] { "Assets/_Project/Audio" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<UnityEngine.Audio.AudioMixer>)
                .FirstOrDefault(m => m.name == mixerName);

            Assert.IsNotNull(mixer, $"Mixer '{mixerName}' not found.");
            Assert.IsTrue(mixer.GetFloat(parameter, out _), $"'{mixerName}' does not expose '{parameter}'.");
        }
    }
}
