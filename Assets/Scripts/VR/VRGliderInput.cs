using QuestSoaring.Aerodynamics;
using UnityEngine;
using UnityEngine.XR;

namespace QuestSoaring.VR
{
    /// <summary>Maps dominant hand controller pose to glider yoke + rudder.</summary>
    public class VRGliderInput : MonoBehaviour
    {
        [SerializeField] GliderInputHub _hub;
        [SerializeField] XRNode _hand = XRNode.RightHand;
        [SerializeField] float _pitchScale = 45f;
        [SerializeField] float _rollScale = 60f;

        public void Bind(GliderInputHub hub) => _hub = hub;

        void Update()
        {
            if (_hub == null) return;
            var device = InputDevices.GetDeviceAtXRNode(_hand);
            if (!device.isValid) return;

            device.TryGetFeatureValue(CommonUsages.deviceRotation, out var rot);
            var euler = rot.eulerAngles;
            var pitch = Mathf.DeltaAngle(0f, euler.x) / _pitchScale;
            var roll = Mathf.DeltaAngle(0f, euler.z) / _rollScale;
            var rudder = 0f;
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out var stick))
                rudder = stick.x;
            _hub.SetStick(pitch, roll, rudder);
        }
    }
}
