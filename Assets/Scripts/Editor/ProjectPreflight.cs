using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace QuestSoaring.EditorTools
{
    public static class ProjectPreflight
    {
        const string MainScene = "Assets/Scenes/Main.unity";

        [MenuItem("Quest Soaring/Preflight Check")]
        public static void Run()
        {
            var ok = true;
            ok &= CheckSceneInBuild();
            ok &= CheckUrp();
            ok &= CheckMetaPackages();
            ok &= CheckMainSceneFile();
            Debug.Log(ok
                ? "[Preflight] All checks passed — press Play on Main.unity"
                : "[Preflight] Fix warnings above, then Play");
        }

        static bool CheckMainSceneFile()
        {
            if (File.Exists(MainScene)) return Pass("Main.unity exists");
            Debug.LogError("[Preflight] Missing Main.unity — open project and run Setup Main Scene");
            return false;
        }

        static bool CheckSceneInBuild()
        {
            foreach (var s in EditorBuildSettings.scenes)
                if (s.path == MainScene && s.enabled) return Pass("Main.unity in build settings");
            Debug.LogWarning("[Preflight] Main.unity not in build settings — run Setup Main Scene");
            return false;
        }

        static bool CheckUrp()
        {
            if (GraphicsSettings.currentRenderPipeline != null)
                return Pass($"URP active: {GraphicsSettings.currentRenderPipeline.name}");
            Debug.LogWarning("[Preflight] URP not assigned — run Quest Soaring → Setup URP Pipeline");
            return false;
        }

        static bool CheckMetaPackages()
        {
            var manifest = File.ReadAllText("Packages/manifest.json");
            if (manifest.Contains("com.meta.xr.sdk.core"))
                return Pass("Meta XR SDK in manifest");
            Debug.LogWarning("[Preflight] Meta XR SDK missing from manifest");
            return false;
        }

        static bool Pass(string msg)
        {
            Debug.Log($"[Preflight] OK: {msg}");
            return true;
        }
    }
}
