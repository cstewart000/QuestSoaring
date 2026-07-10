using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace QuestSoaring.EditorTools
{
    public static class UrpProjectSetup
    {
        const string PipelinePath = "Assets/Settings/QuestSoaringURP.asset";
        const string RendererPath = "Assets/Settings/QuestSoaringURP_Renderer.asset";

        [MenuItem("Quest Soaring/Setup URP Pipeline")]
        public static void SetupPipeline()
        {
            System.IO.Directory.CreateDirectory("Assets/Settings");
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
                Debug.Log("[UrpProjectSetup] Created renderer asset");
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
                Debug.Log("[UrpProjectSetup] Created pipeline asset");
            }

            var so = new SerializedObject(pipeline);
            so.FindProperty("m_RendererDataList").ClearArray();
            so.FindProperty("m_RendererDataList").InsertArrayElementAtIndex(0);
            so.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue = renderer;
            so.ApplyModifiedPropertiesWithoutUndo();

            GraphicsSettings.renderPipelineAsset = pipeline;
            QualitySettings.renderPipeline = pipeline;
            AssetDatabase.SaveAssets();
            Debug.Log("[UrpProjectSetup] URP pipeline assigned — grayscale post will work");
        }
    }
}
