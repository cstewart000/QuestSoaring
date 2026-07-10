using QuestSoaring.Aerodynamics;
using UnityEngine;
using UnityEngine.XR;

namespace QuestSoaring.VR
{
    /// <summary>Keyboard fallback when no XR headset is active (editor testing).</summary>
    public class DesktopFlightInput : MonoBehaviour
    {
        [SerializeField] GliderInputHub _hub;

        public void Bind(GliderInputHub hub) => _hub = hub;

        void Update()
        {
            if (_hub == null || XRSettings.isDeviceActive) return;
            var pitch = Input.GetKey(KeyCode.S) ? 1f : Input.GetKey(KeyCode.W) ? -1f : 0f;
            var roll = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
            var rudder = Input.GetKey(KeyCode.E) ? 1f : Input.GetKey(KeyCode.Q) ? -1f : 0f;
            _hub.SetStick(pitch, roll, rudder);
        }
    }
}
