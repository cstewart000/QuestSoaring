using UnityEngine;

namespace QuestSoaring.Aerodynamics
{
    /// <summary>Merges VR, desktop, and lever inputs into GliderController.</summary>
    public class GliderInputHub : MonoBehaviour
    {
        [SerializeField] GliderController _glider;
        float _pitch, _roll, _rudder, _airBrakes;
        int _logCounter;

        public void Bind(GliderController glider) => _glider = glider;

        public void SetStick(float pitch, float roll, float rudder)
        {
            _pitch = Mathf.Clamp(pitch, -1f, 1f);
            _roll = Mathf.Clamp(roll, -1f, 1f);
            _rudder = Mathf.Clamp(rudder, -1f, 1f);
        }

        public void SetAirBrakes(float normalized) => _airBrakes = Mathf.Clamp01(normalized);

        void Update()
        {
            if (_glider == null) return;
            _glider.SetControlInputs(_pitch, _roll, _rudder, _airBrakes);
            if (++_logCounter % 90 == 0)
                Debug.Log($"[GliderInputHub] p={_pitch:F2} r={_roll:F2} yaw={_rudder:F2} brakes={_airBrakes:F2}");
        }
    }
}
