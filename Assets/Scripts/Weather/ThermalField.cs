using UnityEngine;

namespace QuestSoaring.Weather
{
    /// <summary>Column thermal with strength falloff from core.</summary>
    public struct ThermalColumn
    {
        public Vector2 CenterXZ;
        public float Radius;
        public float CoreClimbMs;

        public float ClimbRateAt(Vector3 worldPos)
        {
            var dx = worldPos.x - CenterXZ.x;
            var dz = worldPos.z - CenterXZ.y;
            var dist = Mathf.Sqrt(dx * dx + dz * dz);
            if (dist > Radius) return 0f;
            var t = 1f - dist / Radius;
            return CoreClimbMs * t * t;
        }
    }

    public class ThermalField : MonoBehaviour
    {
        [SerializeField] ThermalColumn[] _columns = System.Array.Empty<ThermalColumn>();

        public void SetColumns(ThermalColumn[] columns) => _columns = columns;

        public float SampleClimbRate(Vector3 position)
        {
            var total = 0f;
            foreach (var c in _columns) total += c.ClimbRateAt(position);
            if (total > 0.01f)
                Debug.Log($"[ThermalField] climb={total:F2} m/s at {position}");
            return total;
        }

        void FixedUpdate()
        {
            // Applied by ThermalLiftApplier on glider; field is query-only here
        }
    }

    public class ThermalLiftApplier : MonoBehaviour
    {
        [SerializeField] ThermalField _field;
        [SerializeField] Rigidbody _rb;

        void FixedUpdate()
        {
            if (_field == null || _rb == null) return;
            var climb = _field.SampleClimbRate(transform.position);
            if (climb > 0f) _rb.AddForce(Vector3.up * climb * _rb.mass * 0.15f);
        }
    }
}
