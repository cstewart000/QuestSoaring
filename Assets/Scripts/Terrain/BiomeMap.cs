using UnityEngine;

namespace QuestSoaring.Terrain
{
    public enum Biome { Alpine, Forest, Valley, River, Cliff }

    public static class BiomeMap
    {
        public static float ApplyValleysAndRivers(float baseH, float x, float z)
        {
            var valley = ValleyMask(x, z);
            var river = RiverMask(x, z);
            var carve = valley * 75f + river * Mathf.Max(60f, 130f - baseH * 0.12f);
            return Mathf.Max(1.2f, baseH - carve);
        }

        public static float ValleyMask(float x, float z)
        {
            var macro = Mathf.PerlinNoise(x * 0.00085f, z * 0.00085f);
            return Mathf.SmoothStep(0.42f, 0.58f, macro) * (1f - Mathf.SmoothStep(0.68f, 0.82f, macro));
        }

        public static float RiverMask(float x, float z)
        {
            float Channel(float ox, float oz)
            {
                var c = Mathf.PerlinNoise(x * 0.0024f + ox, z * 0.0024f + oz);
                var d = 1f - Mathf.Abs(c - 0.5f) * 9f;
                return Mathf.Clamp01(d);
            }
            var r = Mathf.Max(Channel(12f, 8f), Channel(140f, 95f), Channel(260f, 180f));
            return r * r;
        }

        public static Biome Classify(float x, float z, float y, float slope)
        {
            if (RiverMask(x, z) > 0.35f && y < 140f) return Biome.River;
            if (slope > 0.52f) return Biome.Cliff;
            if (y > 300f) return Biome.Alpine;
            if (y < 135f && slope < 0.38f) return Biome.Valley;
            if (y >= 85f && y <= 295f && slope < 0.42f) return Biome.Forest;
            return y > 200f ? Biome.Alpine : Biome.Valley;
        }

        public static Color GetColor(float x, float z, float y, float slope)
        {
            var b = Classify(x, z, y, slope);
            var speck = Mathf.PerlinNoise(x * 0.07f, z * 0.07f);
            var g = b switch
            {
                Biome.River => 0.03f,
                Biome.Cliff => 0.06f,
                Biome.Valley => 0.2f,
                Biome.Forest => 0.05f + speck * 0.1f,
                Biome.Alpine => 0.86f + speck * 0.14f,
                _ => 0.15f
            };
            return ContrastGray(g);
        }

        static Color ContrastGray(float v)
        {
            v = Mathf.Clamp01((v - 0.5f) * 1.75f + 0.5f);
            return new Color(v, v, v);
        }
    }
}
