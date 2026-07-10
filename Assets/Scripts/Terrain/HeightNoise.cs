using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class HeightNoise
    {
        public const float MaxHeight = 420f;

        public static float Sample(float x, float z, float scale = 0f, int octaves = 0) =>
            TerrainHeightField.Sample(x, z);
    }
}
