using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class TerrainStyle
    {
        public static Color HeightToGray(float y, float slope = 0f)
        {
            var t = Mathf.Pow(Mathf.Clamp01(y / HeightNoise.MaxHeight), 0.88f);
            var g = Mathf.Lerp(0.1f, 0.72f, t);
            g *= 1f - slope * 0.35f;
            return new Color(g, g, g);
        }
    }
}
