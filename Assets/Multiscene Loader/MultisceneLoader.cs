using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class MultisceneLoader : MonoBehaviour {

    public List<SceneAsset> scenes;
    public bool autoReload;

    void Start() {
        if (!Application.isPlaying)
            ReloadScenes();
        else
            LoadScenesInPlayMode();
    }

    #region In-Editor Code
    public void ReloadScenes() {
        RemoveScenesNotInAnyList();
        OpenAllScenes();
    }

    void RemoveScenesNotInAnyList() {
        var loaders = FindAllMultisceneLoaders();
        foreach (var scene in GetAllLoadedScenes()) {
            if (ShouldRemoveScene(scene, loaders))
                EditorSceneManager.CloseScene(scene, true);
        }
    }

    MultisceneLoader[] FindAllMultisceneLoaders() => FindObjectsOfType<MultisceneLoader>();

    List<Scene> GetAllLoadedScenes() {
        List<Scene> scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; ++i)
            scenes.Add(SceneManager.GetSceneAt(i));
        return scenes;
    }

    bool ShouldRemoveScene(Scene scene, MultisceneLoader[] loaders) {
        foreach (var loader in loaders) {
            if (IsLoaderInThisScene(loader, scene) || IsSceneInLoaderSceneAssetList(scene, loader))
                return false;
        }
        return true;
    }

    bool IsLoaderInThisScene(MultisceneLoader loader, Scene scene) => loader.gameObject.scene.path == scene.path;

    bool IsSceneInLoaderSceneAssetList(Scene scene, MultisceneLoader loader) {
        foreach (var sceneAsset in loader.scenes) {
            if (AssetDatabase.GetAssetPath(sceneAsset) == scene.path)
                return true;
        }
        return false;
    }

    void OpenAllScenes() {
        foreach (var scene in scenes) {
            if (scene)
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
        }
    }
    #endregion

    #region Runtime Code
    void LoadScenesInPlayMode() {
        foreach (var scene in scenes) {
            if (!IsSceneAssetLoaded(scene))
                SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
        }
    }

    bool IsSceneAssetLoaded(SceneAsset sceneAsset) {
        for (int i = 0; i < SceneManager.sceneCount; ++i) {
            if (SceneManager.GetSceneAt(i).path == AssetDatabase.GetAssetPath(sceneAsset))
                return true;
        }
        return false;
    }
    #endregion
}
