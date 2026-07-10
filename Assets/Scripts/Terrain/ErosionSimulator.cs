using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class ErosionSimulator
    {
        const int Drops = 3500;
        const int DropSteps = 48;

        public static void Erode(float[,] h, int w, int hgt, int seed)
        {
            var rng = new System.Random(seed);
            for (var d = 0; d < Drops; d++) Drop(h, w, hgt, ref rng);
            for (var t = 0; t < 3; t++) Thermal(h, w, hgt);
            Debug.Log($"[ErosionSimulator] {Drops} drops + thermal on {w}x{hgt}");
        }

        static void Drop(float[,] h, int w, int hgt, ref System.Random rng)
        {
            var x = rng.Next(1, w - 2) + 0.5f;
            var z = rng.Next(1, hgt - 2) + 0.5f;
            var sediment = 0f;
            for (var s = 0; s < DropSteps; s++)
            {
                var ix = Mathf.Clamp(Mathf.FloorToInt(x), 1, w - 2);
                var iz = Mathf.Clamp(Mathf.FloorToInt(z), 1, hgt - 2);
                var gx = h[ix + 1, iz] - h[ix - 1, iz];
                var gz = h[ix, iz + 1] - h[ix, iz - 1];
                var len = Mathf.Sqrt(gx * gx + gz * gz) + 0.001f;
                x -= gx / len * 0.45f;
                z -= gz / len * 0.45f;
                ix = Mathf.Clamp(Mathf.FloorToInt(x), 1, w - 2);
                iz = Mathf.Clamp(Mathf.FloorToInt(z), 1, hgt - 2);
                var nx = ix; var nz = iz; var low = h[ix, iz];
                for (var dz = -1; dz <= 1; dz++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    if (h[ix + dx, iz + dz] < low) { low = h[ix + dx, iz + dz]; nx = ix + dx; nz = iz + dz; }
                }
                if (nx == ix && nz == iz) break;
                var cap = Mathf.Min(0.35f, (h[ix, iz] - h[nx, nz]) * 0.35f);
                h[ix, iz] -= cap;
                sediment += cap;
                if (sediment > 0.5f) { h[nx, nz] += sediment * 0.35f; sediment *= 0.65f; }
            }
        }

        static void Thermal(float[,] h, int w, int hgt)
        {
            for (var z = 1; z < hgt - 1; z++)
            for (var x = 1; x < w - 1; x++)
            {
                var nx = x; var nz = z; var low = h[x, z];
                for (var dz = -1; dz <= 1; dz++)
                for (var dx = -1; dx <= 1; dx++)
                    if (h[x + dx, z + dz] < low) { low = h[x + dx, z + dz]; nx = x + dx; nz = z + dz; }
                var diff = h[x, z] - low;
                if (diff > 0.6f) { var m = diff * 0.45f; h[x, z] -= m; h[nx, nz] += m; }
            }
        }

        public static float[,] FlowAccum(float[,] h, int w, int hgt)
        {
            var flow = new float[w, hgt];
            for (var z = 0; z < hgt; z++)
            for (var x = 0; x < w; x++) flow[x, z] = 1f;
            var order = new System.Collections.Generic.List<(int x, int z, float ht)>();
            for (var z = 0; z < hgt; z++)
            for (var x = 0; x < w; x++) order.Add((x, z, h[x, z]));
            order.Sort((a, b) => b.ht.CompareTo(a.ht));
            foreach (var c in order)
            {
                var nx = c.x; var nz = c.z; var low = h[c.x, c.z];
                for (var dz = -1; dz <= 1; dz++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    var px = c.x + dx; var pz = c.z + dz;
                    if (px < 0 || pz < 0 || px >= w || pz >= hgt) continue;
                    if (h[px, pz] < low) { low = h[px, pz]; nx = px; nz = pz; }
                }
                if (nx != c.x || nz != c.z) flow[nx, nz] += flow[c.x, c.z];
            }
            return flow;
        }
    }
}
