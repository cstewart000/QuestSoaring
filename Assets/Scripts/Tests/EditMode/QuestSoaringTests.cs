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
            var a = HeightNoise.Sample(120f, 340f, 800f, 4);
            var b = HeightNoise.Sample(120f, 340f, 800f, 4);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void HeightNoise_InExpectedRange()
        {
            var h = HeightNoise.Sample(0f, 0f, 800f, 4);
            Assert.That(h, Is.InRange(0f, 400f));
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
}
