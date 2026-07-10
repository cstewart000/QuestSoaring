using UnityEngine;

namespace QuestSoaring.Terrain
{
    public enum Biome { Alpine, Forest, Valley, River, Cliff }

    /// <summary>Palette matched to concept_art/image (1).jpg — high-key white + gray stipple.</summary>
    public static class BiomeMap
    {
        public static float ApplyValleysAndRivers(float baseH, float x, float z)
        {
            var valley = ValleyMask(x, z);
            var river = RiverMask(x, z);
            var carve = valley * 55f + river * Mathf.Max(45f, 100f - baseH * 0.1f);
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
                return Mathf.Clamp01(1f - Mathf.Abs(c - 0.5f) * 9f);
            }
            var r = Mathf.Max(Channel(12f, 8f), Channel(140f, 95f), Channel(260f, 180f));
            return r * r;
        }

        public static bool ForestStipple(float x, float z, float y)
        {
            if (y > 230f || y < 35f) return false;
            var patch = Mathf.PerlinNoise(x * 0.022f, z * 0.022f);
            var fine = Mathf.PerlinNoise(x * 0.19f + 41f, z * 0.19f + 17f);
            return patch > 0.46f && fine > 0.58f;
        }

        public static Biome Classify(float x, float z, float y, float slope)
        {
            if (RiverMask(x, z) > 0.35f && y < 140f) return Biome.River;
            if (ForestStipple(x, z, y)) return Biome.Forest;
            if (y > 280f || slope > 0.48f) return Biome.Alpine;
            if (y < 130f && slope < 0.35f) return Biome.Valley;
            return Biome.Alpine;
        }

        public static Color GetColor(float x, float z, float y, float slope)
        {
            var speck = Mathf.PerlinNoise(x * 0.11f, z * 0.11f);
            if (RiverMask(x, z) > 0.28f && y < 155f) return Gray(0.84f);
            if (ForestStipple(x, z, y)) return Gray(0.44f + speck * 0.08f);
            if (y < 125f && slope < 0.32f) return Gray(0.76f);
            if (y > 270f) return Gray(0.82f + speck * 0.04f);
            if (slope > 0.38f) return Gray(0.74f - slope * 0.06f);
            return Gray(0.8f);
        }

        static Color Gray(float v) => new Color(v, v, v);
    }
}
