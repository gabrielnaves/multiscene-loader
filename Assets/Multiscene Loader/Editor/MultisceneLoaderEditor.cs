using UnityEngine;
using UnityEditor;

namespace GabrielNaves {

    [CustomEditor(typeof(MultisceneLoader))]
    public class MultisceneLoaderEditor : Editor {

        MultisceneLoader loader;
        SerializedProperty autoReloadProperty;

        void OnEnable() {
            loader = target as MultisceneLoader;
            autoReloadProperty = serializedObject.FindProperty("autoReload");
        }

        public override void OnInspectorGUI() {
            var loader = target as MultisceneLoader;

            if (GUILayout.Button("Add Scene", GetAddButtonStyle()))
                AddEmptyScene();
            for (int i = 0; i < loader.scenes.Count; ++i) {
                EditorGUILayout.BeginHorizontal();
                ShowSceneObjectField(i);
                if (GUILayout.Button("remove", GetRemoveButtonStyle(), GUILayout.Width(70)))
                    RemoveSceneAt(i--);
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Open Scenes"))
                loader.ReloadScenes();
            EditorGUILayout.PropertyField(autoReloadProperty);
            serializedObject.ApplyModifiedProperties();
            if (loader.autoReload) {
                EditorGUILayout.HelpBox("Scenes reload when they're set or removed, but undo-ing changes won't reload the currently selected scenes.", MessageType.Info);
                EditorGUILayout.HelpBox("Removing a scene with unsaved changes will result in a loss of said changes.", MessageType.Warning);
            }
        }

        GUIStyle GetAddButtonStyle() {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = new Color(0.23f, 0.62f, 0.28f);
            return style;
        }

        GUIStyle GetRemoveButtonStyle() {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            return style;
        }

        void AddEmptyScene() {
            Undo.RecordObject(target, "scenes");
            loader.scenes.Add(null);
            EditorUtility.SetDirty(target);
        }

        void RemoveSceneAt(int index) {
            Undo.RecordObject(target, "scenes");
            loader.scenes.RemoveAt(index);
            EditorUtility.SetDirty(target);
            if (loader.autoReload)
                loader.ReloadScenes();
        }

        void ShowSceneObjectField(int index) {
            var currentScene = loader.scenes[index];
            var selectedScene = EditorGUILayout.ObjectField(currentScene, typeof(SceneAsset), allowSceneObjects: false) as SceneAsset;
            if (selectedScene != currentScene) {
                Undo.RecordObject(target, "scenes");
                loader.scenes[index] = selectedScene;
                EditorUtility.SetDirty(target);
                if (loader.autoReload)
                    loader.ReloadScenes();
            }
        }
    }
}
