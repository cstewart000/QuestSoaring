using UnityEngine;

namespace QuestSoaring.VR
{
    /// <summary>Physics-hand lever: 0–1 value from local rotation on grab axis.</summary>
    public class InteractableLever : MonoBehaviour
    {
        [SerializeField] Vector3 _localAxis = Vector3.forward;
        [SerializeField] float _minAngle = -30f;
        [SerializeField] float _maxAngle = 30f;
        [SerializeField] bool _grabbed;

        public float NormalizedValue { get; private set; }

        public void SetGrabbed(bool grabbed)
        {
            _grabbed = grabbed;
            Debug.Log($"[InteractableLever] {name} grabbed={grabbed}");
        }

        void Update()
        {
            var angle = Vector3.Dot(transform.localEulerAngles, _localAxis.normalized);
            if (angle > 180f) angle -= 360f;
            NormalizedValue = Mathf.InverseLerp(_minAngle, _maxAngle, angle);
        }

        public void SnapTo(float normalized)
        {
            var angle = Mathf.Lerp(_minAngle, _maxAngle, Mathf.Clamp01(normalized));
            transform.localRotation = Quaternion.AngleAxis(angle, _localAxis);
            Debug.Log($"[InteractableLever] {name} snapped to {normalized:F2}");
        }
    }
}
