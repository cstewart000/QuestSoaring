using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class HeightNoise
    {
        public const float DefaultScale = 680f;
        public const int DefaultOctaves = 5;
        public const float MaxHeight = 460f;

        public static float Sample(float x, float z, float scale = DefaultScale, int octaves = DefaultOctaves)
        {
            return BiomeMap.ApplyValleysAndRivers(BaseHeight(x, z, scale, octaves), x, z);
        }

        static float BaseHeight(float x, float z, float scale, int octaves)
        {
            var h = 0f;
            var amp = 1f;
            var freq = 1f / scale;
            var ampSum = 0f;
            for (var i = 0; i < octaves; i++)
            {
                var p = Mathf.PerlinNoise(x * freq, z * freq);
                var ridged = 1f - Mathf.Abs(p * 2f - 1f);
                var n = ridged * 0.55f + p * 0.45f;
                h += n * amp;
                ampSum += amp;
                amp *= 0.52f;
                freq *= 2.05f;
            }
            var norm = Mathf.Pow(Mathf.Clamp01(h / ampSum), 1.04f);
            return norm * MaxHeight;
        }
    }
}
