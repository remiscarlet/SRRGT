using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {
    public const string MainMenuScene = "Main Menu";
    public const string GameplayScene = "Gameplay";
    public const string EditorScene = "Editor";

    public LightingSettings lightingSettings;

    private void LoadScene(string sceneName) {
        SceneManager.LoadSceneAsync(sceneName);
        Lightmapping.lightingSettings = lightingSettings;
    }

    public void LoadMainMenuScene() {
        LoadScene(MainMenuScene);
    }

    public void LoadGameplayScene() {
        LoadScene(GameplayScene);
    }


}
