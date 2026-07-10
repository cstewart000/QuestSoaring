using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace QuestSoaring.SceneSetup
{
    /// <summary>Applies grayscale URP post-processing for MVP art direction.</summary>
    public static class GrayscalePostSetup
    {
        public static void Apply(Transform parent)
        {
            if (!RenderUtil.IsUrpActive)
            {
                Debug.LogWarning("[GrayscalePostSetup] URP not active — run Quest Soaring → Setup URP Pipeline");
                return;
            }
            var go = new GameObject("GrayscaleVolume");
            go.transform.SetParent(parent);
            var volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            var sat = volume.profile.Add<ColorAdjustments>(true);
            sat.saturation.Override(-85f);
            sat.postExposure.Override(0.1f);
            Debug.Log("[GrayscalePostSetup] Grayscale volume active");
        }
    }
}
