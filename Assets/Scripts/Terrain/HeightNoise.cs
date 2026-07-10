using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class HeightNoise
    {
        public const float DefaultScale = 620f;
        public const int DefaultOctaves = 5;
        public const float MaxHeight = 480f;

        public static float Sample(float x, float z, float scale = DefaultScale, int octaves = DefaultOctaves)
        {
            var h = 0f;
            var amp = 1f;
            var freq = 1f / scale;
            var ampSum = 0f;
            for (var i = 0; i < octaves; i++)
            {
                var n = Mathf.PerlinNoise(x * freq, z * freq);
                n = 1f - Mathf.Abs(n * 2f - 1f);
                h += n * n * amp;
                ampSum += amp;
                amp *= 0.52f;
                freq *= 2.1f;
            }
            var norm = h / ampSum;
            norm = Mathf.Pow(Mathf.Clamp01(norm), 1.28f);
            return norm * MaxHeight;
        }
    }
}
