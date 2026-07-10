using QuestSoaring.Terrain;
using UnityEngine;

namespace QuestSoaring.Weather
{
    /// <summary>Ridge lift from slope facing into wind (heuristic).</summary>
    public static class RidgeLiftModel
    {
        public static float CalcLiftMs(Vector3 wind, Vector3 terrainNormal, float windFactor = 1f)
        {
            var w = new Vector2(wind.x, wind.z);
            if (w.sqrMagnitude < 0.01f) return 0f;
            var n = terrainNormal.normalized;
            if (n.y < 0.05f) return 0f;
            var slope = 1f - n.y;
            var facing = Vector2.Dot(w.normalized, new Vector2(n.x, n.z).normalized);
            if (facing <= 0f) return 0f;
            return wind.magnitude * slope * facing * windFactor;
        }

        public static Vector3 SampleTerrainNormal(float x, float z, float sampleStep = 8f)
        {
            var hL = HeightNoise.Sample(x - sampleStep, z);
            var hR = HeightNoise.Sample(x + sampleStep, z);
            var hD = HeightNoise.Sample(x, z - sampleStep);
            var hU = HeightNoise.Sample(x, z + sampleStep);
            return new Vector3(hL - hR, 2f * sampleStep, hD - hU).normalized;
        }
    }
}
