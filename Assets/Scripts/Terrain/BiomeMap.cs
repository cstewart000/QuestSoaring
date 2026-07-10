using UnityEngine;

namespace QuestSoaring.Terrain
{
    public enum Biome { Alpine, Forest, Valley, River, Cliff }

    public static class BiomeMap
    {
        public static float SampleFlow(float x, float z)
        {
            var cx = Mathf.FloorToInt(x / TerrainHeightField.ChunkSize);
            var cz = Mathf.FloorToInt(z / TerrainHeightField.ChunkSize);
            return TerrainHeightField.GetChunk(new Vector2Int(cx, cz)).SampleFlow(x, z);
        }

        public static bool ForestPatch(float x, float z, float y)
        {
            if (y > 220f || y < 40f) return false;
            var patch = Mathf.PerlinNoise(x * 0.018f, z * 0.018f);
            return patch > 0.5f && SampleFlow(x, z) < 6f;
        }

        public static Color GetColor(float x, float z, float y, float slope, float flow)
        {
            var speck = Mathf.PerlinNoise(x * 0.09f, z * 0.09f);
            if (flow > 10f && y < 200f) return Gray(0.84f);
            if (ForestPatch(x, z, y)) return Gray(0.44f + speck * 0.08f);
            if (y < 110f && slope < 0.28f) return Gray(0.76f);
            if (y > 260f) return Gray(0.82f + speck * 0.04f);
            if (slope > 0.42f) return Gray(0.72f - slope * 0.05f);
            return Gray(0.8f);
        }

        static Color Gray(float v) => new Color(v, v, v);
    }
}
