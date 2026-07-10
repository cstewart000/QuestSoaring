using NUnit.Framework;
using QuestSoaring.Aerodynamics;
using QuestSoaring.Terrain;
using QuestSoaring.UI;
using QuestSoaring.Weather;
using UnityEngine;

namespace QuestSoaring.Tests
{
    public class GliderAeroModelTests
    {
        GliderAeroModel _model;

        [SetUp]
        public void SetUp() => _model = GliderAeroModel.Beginner;

        [Test]
        public void PerformanceProfile_HasHigherGlideRatio()
        {
            Assert.Greater(GliderProfile.Performance.Aero.BestGlideRatio,
                GliderProfile.Beginner.Aero.BestGlideRatio);
        }
        [Test]
        public void LiftIncreasesWithPositiveAoA()
        {
            var low = _model.Compute(new Vector3(0, 0, 25f), 2f, 0f);
            var high = _model.Compute(new Vector3(0, 0, 25f), 8f, 0f);
            Assert.Greater(high.Lift, low.Lift);
        }

        [Test]
        public void DragIncreasesWithAirBrakes()
        {
            var clean = _model.Compute(new Vector3(0, 0, 30f), 5f, 0f);
            var brakes = _model.Compute(new Vector3(0, 0, 30f), 5f, 1f);
            Assert.Greater(brakes.Drag, clean.Drag);
        }

        [Test]
        public void StallDetectedAtHighAoAAndLowSpeed()
        {
            var result = _model.Compute(new Vector3(0, 0, 8f), 18f, 0f);
            Assert.IsTrue(result.Stalled);
        }

        [Test]
        public void AngleOfAttack_MatchesForwardFlight()
        {
            var forward = Vector3.forward;
            var up = Vector3.up;
            var vel = Quaternion.Euler(5f, 0f, 0f) * Vector3.forward * 20f;
            var aoa = _model.AngleOfAttackDeg(vel, forward, up);
            Assert.That(aoa, Is.InRange(4f, 6f));
        }
    }

    public class ThermalFieldTests
    {
        [Test]
        public void CoreHasMaxClimb()
        {
            var col = new ThermalColumn { CenterXZ = Vector2.zero, Radius = 100f, CoreClimbMs = 3f };
            Assert.That(col.ClimbRateAt(Vector3.zero), Is.EqualTo(3f).Within(0.01f));
        }

        [Test]
        public void OutsideRadiusZeroClimb()
        {
            var col = new ThermalColumn { CenterXZ = Vector2.zero, Radius = 50f, CoreClimbMs = 2f };
            Assert.AreEqual(0f, col.ClimbRateAt(new Vector3(100f, 0f, 0f)));
        }
    }

    public class TerrainHeightTests
    {
        [Test]
        public void HeightNoise_IsDeterministic()
        {
            var a = HeightNoise.Sample(120f, 340f);
            var b = HeightNoise.Sample(120f, 340f);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void HeightNoise_InExpectedRange()
        {
            var h = HeightNoise.Sample(0f, 0f);
            Assert.That(h, Is.InRange(1f, HeightNoise.MaxHeight));
        }
    }

    public class BiomeMapTests
    {
        [Test]
        public void FlowAndHeight_SampleConsistently()
        {
            var h = HeightNoise.Sample(200f, 200f);
            var flow = BiomeMap.SampleFlow(200f, 200f);
            Assert.That(h, Is.GreaterThan(1f));
            Assert.That(flow, Is.GreaterThan(0f));
        }
    }

    public class FlightInstrumentsTests
    {
        [Test]
        public void Vario_PositiveWhenClimbing()
        {
            var vario = FlightInstruments.CalcVarioMps(110f, 100f, 1f);
            Assert.AreEqual(10f, vario, 0.01f);
        }

        [Test]
        public void Vario_NegativeWhenSinking()
        {
            var vario = FlightInstruments.CalcVarioMps(90f, 100f, 2f);
            Assert.AreEqual(-5f, vario, 0.01f);
        }
    }

    public class RidgeLiftTests
    {
        [Test]
        public void RidgeLift_ZeroWhenWindAwayFromSlope()
        {
            var wind = new Vector3(0f, 0f, 10f);
            var normal = new Vector3(0f, 0.7f, 0.7f).normalized;
            Assert.AreEqual(0f, RidgeLiftModel.CalcLiftMs(wind, normal));
        }

        [Test]
        public void RidgeLift_PositiveWhenWindIntoSlope()
        {
            var wind = new Vector3(10f, 0f, 0f);
            var normal = new Vector3(-0.5f, 0.5f, 0f).normalized;
            Assert.Greater(RidgeLiftModel.CalcLiftMs(wind, normal), 0f);
        }
    }

    public class WindFieldTests
    {
        [Test]
        public void Wind_IncreasesWithAltitude()
        {
            var low = WindField.Sample(new Vector3(0f, 100f, 0f)).Velocity.magnitude;
            var high = WindField.Sample(new Vector3(0f, 1500f, 0f)).Velocity.magnitude;
            Assert.Greater(high, low);
        }
    }
}
