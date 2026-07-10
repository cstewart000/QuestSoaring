using UnityEngine;

namespace QuestSoaring.Aerodynamics
{
    /// <summary>Pure heuristic aero math (Condor-inspired). Testable without Unity scene.</summary>
    public struct GliderAeroModel
    {
        public float WingArea;
        public float Mass;
        public float ClMax;
        public float MinSinkSpeed;
        public float BestGlideRatio;

        public static GliderAeroModel Beginner => new()
        {
            WingArea = 12f, Mass = 300f, ClMax = 1.4f,
            MinSinkSpeed = 18f, BestGlideRatio = 28f
        };

        public float AngleOfAttackDeg(Vector3 velocity, Vector3 forward, Vector3 up)
        {
            var local = Quaternion.Inverse(Quaternion.LookRotation(forward, up)) * velocity;
            return Mathf.Atan2(-local.y, local.z) * Mathf.Rad2Deg;
        }

        public float LiftCoefficient(float aoaDeg)
        {
            const float slope = 0.09f;
            var cl = slope * aoaDeg;
            if (aoaDeg > 12f) cl -= (aoaDeg - 12f) * 0.04f; // pre-stall softening
            return Mathf.Clamp(cl, -0.6f, ClMax);
        }

        public float DragCoefficient(float cl)
        {
            var induced = cl * cl / (Mathf.PI * 8f * 0.82f);
            return 0.012f + induced;
        }

        public AeroForces Compute(Vector3 velocity, float aoaDeg, float airBrakeExtension)
        {
            var speed = velocity.magnitude;
            if (speed < 0.5f) return default;

            var rho = 1.225f;
            var q = 0.5f * rho * speed * speed;
            var cl = LiftCoefficient(aoaDeg);
            var cd = DragCoefficient(cl) + airBrakeExtension * 0.08f;
            var lift = q * WingArea * cl;
            var drag = q * WingArea * cd;
            Debug.Log($"[Aero] spd={speed:F1} aoa={aoaDeg:F1} cl={cl:F2} lift={lift:F0} drag={drag:F0}");
            return new AeroForces { Lift = lift, Drag = drag, Stalled = aoaDeg > 14f && speed < MinSinkSpeed * 0.65f };
        }
    }

    public struct AeroForces
    {
        public float Lift;
        public float Drag;
        public bool Stalled;
    }
}
