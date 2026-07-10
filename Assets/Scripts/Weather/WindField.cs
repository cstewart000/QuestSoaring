using UnityEngine;

namespace QuestSoaring.Weather
{
    /// <summary>Altitude-layered wind for ridge lift and drift.</summary>
    public struct WindSample
    {
        public Vector3 Velocity;
        public float RidgeFactor;
    }

    public static class WindField
    {
        public static WindSample Sample(Vector3 position, float baseSpeed = 6f)
        {
            var alt = position.y;
            var layer = Mathf.Clamp01(alt / 2000f);
            var dir = Quaternion.Euler(0f, 245f + layer * 20f, 0f) * Vector3.forward;
            var speed = baseSpeed * (0.6f + layer * 0.8f);
            var gust = Mathf.PerlinNoise(position.x * 0.002f, position.z * 0.002f) * 2f;
            return new WindSample
            {
                Velocity = dir * (speed + gust),
                RidgeFactor = 0.35f + layer * 0.25f
            };
        }
    }

    public class WindDriftApplier : MonoBehaviour
    {
        [SerializeField] Rigidbody _rb;

        public void Init(Rigidbody rb) => _rb = rb;

        void FixedUpdate()
        {
            if (_rb == null) return;
            var wind = WindField.Sample(transform.position);
            _rb.AddForce(wind.Velocity * _rb.mass * 0.02f);
        }
    }

    public class RidgeLiftApplier : MonoBehaviour
    {
        [SerializeField] Rigidbody _rb;

        public void Init(Rigidbody rb) => _rb = rb;

        void FixedUpdate()
        {
            if (_rb == null) return;
            var p = transform.position;
            var wind = WindField.Sample(p);
            var normal = RidgeLiftModel.SampleTerrainNormal(p.x, p.z);
            var lift = RidgeLiftModel.CalcLiftMs(wind.Velocity, normal, wind.RidgeFactor);
            if (lift > 0.05f)
            {
                _rb.AddForce(Vector3.up * lift * _rb.mass * 0.12f);
                Debug.Log($"[RidgeLift] +{lift:F2} m/s at {p}");
            }
        }
    }
}
