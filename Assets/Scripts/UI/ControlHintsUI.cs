using UnityEngine;
using UnityEngine.UI;

namespace QuestSoaring.UI
{
    /// <summary>Desktop control overlay shown when XR is inactive.</summary>
    public class ControlHintsUI : MonoBehaviour
    {
        [SerializeField] Text _label;

        public static ControlHintsUI Create(Transform parent)
        {
            var go = new GameObject("ControlHints");
            go.transform.SetParent(parent, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var rect = textGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(16f, 16f);
            rect.sizeDelta = new Vector2(520f, 120f);
            var text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 18;
            text.color = new Color(0.85f, 0.88f, 0.9f, 0.9f);
            text.alignment = TextAnchor.LowerLeft;
            var hints = go.AddComponent<ControlHintsUI>();
            hints._label = text;
            hints.Refresh();
            Debug.Log("[ControlHintsUI] Desktop hints visible");
            return hints;
        }

        void Update()
        {
            if (Time.frameCount % 60 == 0) Refresh();
        }

        void Refresh()
        {
            if (_label == null) return;
            var xr = UnityEngine.XR.XRSettings.isDeviceActive;
            _label.text = xr
                ? "VR: Tilt controller = yoke | Stick X = rudder | Grab levers"
                : "W/S pitch | A/D roll | Q/E rudder | Air brake lever (right)";
        }
    }
}
