using UnityEngine;

namespace QuestSoaring.Terrain
{
    /// <summary>Continental uplift + oriented ranges before erosion.</summary>
    public static class NaturalUplift
    {
        public static float Sample(float x, float z)
        {
            var warpX = x + Mathf.PerlinNoise(x * 0.00028f, z * 0.00028f) * 420f;
            var warpZ = z + Mathf.PerlinNoise(x * 0.00028f + 90f, z * 0.00028f + 40f) * 420f;
            var continent = Mathf.PerlinNoise(warpX * 0.00022f, warpZ * 0.00022f);
            var ridge = 1f - Mathf.Abs(Mathf.PerlinNoise(warpX * 0.00085f, warpZ * 0.00085f) * 2f - 1f);
            var h = (continent * 0.38f + ridge * 0.62f) * 360f + 50f;
            return h;
        }
    }
}
