using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class TerrainStyle
    {
        public static Color SurfaceColor(float x, float z, float y, float slope) =>
            BiomeMap.GetColor(x, z, y, slope);
    }
}
