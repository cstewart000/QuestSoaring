using QuestSoaring.Aerodynamics;
using QuestSoaring.UI;
using QuestSoaring.VR;
using QuestSoaring.Weather;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSoaring.SceneSetup
{
    public static class GliderFactory
    {
        public static GliderRig Create(Transform parent, ThermalField thermals)
        {
            var go = new GameObject("Glider");
            go.transform.SetParent(parent);
            go.transform.position = new Vector3(0f, 600f, 0f);
            go.transform.rotation = Quaternion.Euler(-5f, 0f, 0f);

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 300f;
            rb.drag = 0.02f;
            rb.angularDrag = 2f;
            rb.velocity = go.transform.forward * 28f;

            var controller = go.AddComponent<GliderController>();
            var hub = go.AddComponent<GliderInputHub>();
            hub.Bind(controller);

            var lift = go.AddComponent<ThermalLiftApplier>();
            lift.Init(thermals, rb);

            GliderMeshBuilder.Build(go.transform);
            BuildCockpit(go.transform, hub, rb);
            Debug.Log("[GliderFactory] Glider rig ready at 600m");
            return new GliderRig { Root = go.transform, Body = rb, Hub = hub };
        }

        static void BuildCockpit(Transform glider, GliderInputHub hub, Rigidbody rb)
        {
            var cockpit = new GameObject("Cockpit").transform;
            cockpit.SetParent(glider, false);
            cockpit.localPosition = new Vector3(0f, 0.3f, 0.4f);

            var camGo = new GameObject("PilotCamera");
            camGo.transform.SetParent(cockpit, false);
            camGo.transform.localPosition = Vector3.zero;
            camGo.AddComponent<Camera>().tag = "MainCamera";
            camGo.AddComponent<AudioListener>();

            var inputGo = new GameObject("VRInput");
            inputGo.transform.SetParent(cockpit, false);
            var vr = inputGo.AddComponent<VRGliderInput>();
            vr.Bind(hub);
            var desktop = inputGo.AddComponent<DesktopFlightInput>();
            desktop.Bind(hub);

            var leverGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leverGo.name = "AirBrakeLever";
            leverGo.transform.SetParent(cockpit, false);
            leverGo.transform.localPosition = new Vector3(0.35f, -0.1f, 0.1f);
            leverGo.transform.localScale = new Vector3(0.03f, 0.15f, 0.03f);
            Object.Destroy(leverGo.GetComponent<Collider>());
            var lever = leverGo.AddComponent<InteractableLever>();
            var binding = leverGo.AddComponent<AirBrakeLeverBinding>();
            binding.Bind(lever, hub);

            BuildInstrumentPanel(cockpit, rb);
        }

        static void BuildInstrumentPanel(Transform cockpit, Rigidbody rb)
        {
            var canvasGo = new GameObject("Instruments");
            canvasGo.transform.SetParent(cockpit, false);
            canvasGo.transform.localPosition = new Vector3(0f, -0.05f, 0.55f);
            canvasGo.transform.localScale = Vector3.one * 0.001f;
            var rect = canvasGo.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500f, 200f);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasGo.AddComponent<CanvasScaler>();
            var panel = canvasGo.AddComponent<Image>();
            panel.color = new Color(0.1f, 0.1f, 0.12f, 0.85f);

            var vario = MakeLabel(canvasGo.transform, "VARIO", new Vector2(0f, 40f));
            var alt = MakeLabel(canvasGo.transform, "ALT", new Vector2(0f, 0f));
            var spd = MakeLabel(canvasGo.transform, "SPD", new Vector2(0f, -40f));
            var instr = canvasGo.AddComponent<FlightInstruments>();
            instr.Bind(rb, vario, alt, spd);
        }

        static Text MakeLabel(Transform parent, string label, Vector2 pos)
        {
            var go = new GameObject(label);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400f, 60f);
            rect.anchoredPosition = pos;
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.9f, 0.92f, 0.88f);
            text.text = label;
            return text;
        }
    }

    public struct GliderRig
    {
        public Transform Root;
        public Rigidbody Body;
        public GliderInputHub Hub;
    }
}
