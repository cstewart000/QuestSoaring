using UnityEngine;

namespace QuestSoaring.Aerodynamics
{
    [RequireComponent(typeof(Rigidbody))]
    public class GliderController : MonoBehaviour
    {
        [SerializeField] float _pitchInput;
        [SerializeField] float _rollInput;
        [SerializeField] float _rudderInput;
        [SerializeField] float _airBrakes;

        Rigidbody _rb;
        GliderAeroModel _model = GliderAeroModel.Beginner;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.mass = _model.Mass;
            Debug.Log("[GliderController] Awake — beginner profile loaded");
        }

        public void SetControlInputs(float pitch, float roll, float rudder, float airBrakes)
        {
            _pitchInput = Mathf.Clamp(pitch, -1f, 1f);
            _rollInput = Mathf.Clamp(roll, -1f, 1f);
            _rudderInput = Mathf.Clamp(rudder, -1f, 1f);
            _airBrakes = Mathf.Clamp01(airBrakes);
        }

        void FixedUpdate()
        {
            ApplyControlSurfaces();
            ApplyAerodynamics();
        }

        void ApplyControlSurfaces()
        {
            var pitchTorque = _pitchInput * 8000f;
            var rollTorque = _rollInput * 12000f;
            var yawTorque = _rudderInput * 4000f;
            _rb.AddRelativeTorque(new Vector3(pitchTorque, yawTorque, -rollTorque));
        }

        void ApplyAerodynamics()
        {
            var vel = _rb.velocity;
            var aoa = _model.AngleOfAttackDeg(vel, transform.forward, transform.up);
            var forces = _model.Compute(vel, aoa, _airBrakes);
            var liftDir = Vector3.Cross(_rb.velocity.normalized, transform.right).normalized;
            if (liftDir.sqrMagnitude < 0.01f) liftDir = transform.up;
            _rb.AddForce(liftDir * forces.Lift);
            _rb.AddForce(-vel.normalized * forces.Drag);
            if (forces.Stalled) _rb.AddRelativeTorque(Vector3.right * 5000f);
        }
    }
}
