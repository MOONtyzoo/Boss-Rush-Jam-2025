using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // Enum names must match the scene names exactly
    // These scenes must also be present in the build profile
    public enum Scene {
        MainMenu,
        Game,
        RoomSpinTest
    }

    public static void Load(Scene scene) {
        SceneManager.LoadScene(scene.ToString());
    }
}