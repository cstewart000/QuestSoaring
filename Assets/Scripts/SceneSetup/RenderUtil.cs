using UnityEngine;
using UnityEngine.Rendering;

namespace QuestSoaring.SceneSetup
{
    public static class RenderUtil
    {
        public static Material CreateLitMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Universal Render Pipeline/Simple Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Unlit/Color");
            if (shader == null)
                Debug.LogWarning("[RenderUtil] No lit shader found — mesh may render pink");
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else mat.color = color;
            return mat;
        }

        public static bool IsUrpActive =>
            GraphicsSettings.currentRenderPipeline != null;
    }
}
