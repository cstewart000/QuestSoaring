using UnityEngine;
using UnityEngine.UI;

namespace QuestSoaring.UI
{
    /// <summary>Vario, altimeter, airspeed readout for cockpit panel.</summary>
    public class FlightInstruments : MonoBehaviour
    {
        [SerializeField] Rigidbody _gliderBody;
        [SerializeField] Text _varioText;
        [SerializeField] Text _altText;
        [SerializeField] Text _speedText;
        float _lastAlt;
        int _logCounter;

        public void Bind(Rigidbody body, Text vario, Text alt, Text speed)
        {
            _gliderBody = body;
            _varioText = vario;
            _altText = alt;
            _speedText = speed;
            _lastAlt = body != null ? body.position.y : 0f;
        }

        public static float CalcVarioMps(float altNow, float altPrev, float dt)
        {
            if (dt <= 0f) return 0f;
            return (altNow - altPrev) / dt;
        }

        void Update()
        {
            if (_gliderBody == null) return;
            var dt = Time.deltaTime;
            var alt = _gliderBody.position.y;
            var vario = CalcVarioMps(alt, _lastAlt, dt);
            _lastAlt = alt;
            var speed = _gliderBody.velocity.magnitude;
            if (_varioText) _varioText.text = $"VARIO {vario:+0.0;-0.0} m/s";
            if (_altText) _altText.text = $"ALT {alt:0} m";
            if (_speedText) _speedText.text = $"SPD {speed:0} m/s";
            if (++_logCounter % 120 == 0)
                Debug.Log($"[FlightInstruments] vario={vario:F1} alt={alt:F0} spd={speed:F0}");
        }
    }
}
