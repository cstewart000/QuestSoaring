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
        GliderProfile _profile = GliderProfile.Beginner;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            ApplyProfileMass();
        }

        public void SetProfile(GliderProfile profile)
        {
            _profile = profile;
            ApplyProfileMass();
            Debug.Log($"[GliderController] Profile: {profile.Name}");
        }

        void ApplyProfileMass()
        {
            if (_rb != null) _rb.mass = _profile.Aero.Mass;
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
            var pitchTorque = _pitchInput * _profile.PitchAuthority;
            var rollTorque = _rollInput * _profile.RollAuthority;
            var yawTorque = _rudderInput * 4000f;
            _rb.AddRelativeTorque(new Vector3(pitchTorque, yawTorque, -rollTorque));
        }

        void ApplyAerodynamics()
        {
            var model = _profile.Aero;
            var vel = _rb.velocity;
            var aoa = model.AngleOfAttackDeg(vel, transform.forward, transform.up);
            var forces = model.Compute(vel, aoa, _airBrakes);
            var liftDir = Vector3.Cross(_rb.velocity.normalized, transform.right).normalized;
            if (liftDir.sqrMagnitude < 0.01f) liftDir = transform.up;
            _rb.AddForce(liftDir * forces.Lift);
            _rb.AddForce(-vel.normalized * forces.Drag);
            if (forces.Stalled) _rb.AddRelativeTorque(Vector3.right * 5000f);
        }
    }
}
