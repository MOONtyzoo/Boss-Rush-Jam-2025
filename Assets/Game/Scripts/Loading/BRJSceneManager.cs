namespace BRJ.SceneManagement
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// An enum representing all the different scenes you can transition to
    /// </summary>
    public enum Scenes {
        // ! Update this list with all scenes
        MainMenuFinalOutline,
        Game,
        RoomSpinTest,
        Level1
    }

    /// <summary>
    /// A custom scene manager to interact with our loading system.
    /// </summary>
    public static class BRJSceneManager
    {
        public static string SceneToLoad { get; private set; }

        public static bool UseSpeedrunMode { get; private set; }
        public static bool UseHardcoreMode { get; private set; }

        /// <summary>
        /// Loads the scene specified asynchronously by a build index by first loading a "loading" scene.
        /// </summary>
        /// <param name="sceneBuildIndex">An int representing the build index of the scene.</param>
        /// <param name="enableSpeedRunMode">(Optional) - Add speed run mode</param>
        /// <param name="enableHardcoreMode">(Optional) - Add hardcore mode</param>
        public static void LoadSceneAsync(int sceneBuildIndex, bool? enableSpeedRunMode = null, bool? enableHardcoreMode = null)
        {
            GoToScene(SceneManager.GetSceneByBuildIndex(sceneBuildIndex).name, enableSpeedRunMode, enableHardcoreMode);
        }

        /// <summary>
        /// Loads the scene specified asynchronously by a scene name by first loading a "loading" scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene as a string.</param>
        /// <param name="enableSpeedRunMode">(Optional) - Add speed run mode</param>
        /// <param name="enableHardcoreMode">(Optional) - Add hardcore mode</param>
        public static void LoadSceneAsync(string sceneName, bool? enableSpeedRunMode = null, bool? enableHardcoreMode = null)
        {
            GoToScene(sceneName, enableSpeedRunMode, enableHardcoreMode);
        }

        /// <summary>
        /// Loads the scene specified asynchronously by a scene name by first loading a "loading" scene.
        /// </summary>
        /// <param name="sceneName">The sceneName from an enum selection</param>
        /// <param name="enableSpeedRunMode">(Optional) - Add speed run mode</param>
        /// <param name="enableHardcoreMode">(Optional) - Add hardcore mode</param>
        public static void LoadSceneAsync(Scenes sceneName, bool? enableSpeedRunMode = null, bool? enableHardcoreMode = null)
        {
            GoToScene(sceneName.ToString(), enableSpeedRunMode, enableHardcoreMode);
        }

        /// <summary>
        /// Clears "SceneToLoad" to prepare for another scene needing loading.
        /// </summary>
        public static void ClearSceneToLoad()
        {
            SceneToLoad = default;
        }

        private static void GoToScene(string scene, bool? enableSpeedRunMode, bool? enableHardcoreMode)
        {
            SceneToLoad = scene;

            UseSpeedrunMode = enableSpeedRunMode ?? UseSpeedrunMode;
            UseHardcoreMode = enableHardcoreMode ?? UseHardcoreMode;

            SceneManager.LoadSceneAsync("Loading");
        }
    }
}