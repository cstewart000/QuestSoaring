using QuestSoaring.SceneSetup;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace QuestSoaring.EditorTools
{
    public static class MainSceneSetup
    {
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string OvrRigPath = "Packages/com.meta.xr.sdk.core/Prefabs/OVRCameraRig.prefab";

        [MenuItem("Quest Soaring/Setup Main Scene")]
        public static void SetupMainScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var bootstrapGo = new GameObject("SceneBootstrap");
            var bootstrap = bootstrapGo.AddComponent<SceneBootstrap>();
            bootstrap.Build();

            var glider = GameObject.Find("Glider");
            if (glider && AssetDatabase.LoadAssetAtPath<GameObject>(OvrRigPath) is GameObject rigPrefab)
            {
                var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPrefab);
                rig.transform.SetParent(glider.transform, false);
                rig.transform.localPosition = new Vector3(0f, 0.3f, 0.4f);
                Object.DestroyImmediate(GameObject.Find("PilotCamera"));
                Debug.Log("[MainSceneSetup] OVRCameraRig parented to glider");
            }

            System.IO.Directory.CreateDirectory("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            Debug.Log($"[MainSceneSetup] Saved {ScenePath}");
        }

        static void AddSceneToBuildSettings(string path)
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var s in scenes)
                if (s.path == path) return;
            var list = new EditorBuildSettingsScene[scenes.Length + 1];
            scenes.CopyTo(list, 0);
            list[^1] = new EditorBuildSettingsScene(path, true);
            EditorBuildSettings.scenes = list;
        }
    }
}
