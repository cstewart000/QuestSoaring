using System.Collections.Generic;
using UnityEngine;

namespace QuestSoaring.Terrain
{
    public class ChunkHeightData
    {
        public float[,] Heights;
        public float[,] Flow;
        public float OriginX, OriginZ, CellSize;
        public int Width, Height;

        public float SampleHeight(float wx, float wz)
        {
            var lx = (wx - OriginX) / CellSize;
            var lz = (wz - OriginZ) / CellSize;
            return Bilinear(Heights, lx, lz, Width, Height);
        }

        public float SampleFlow(float wx, float wz)
        {
            var lx = (wx - OriginX) / CellSize;
            var lz = (wz - OriginZ) / CellSize;
            return Bilinear(Flow, lx, lz, Width, Height);
        }

        static float Bilinear(float[,] g, float x, float z, int w, int h)
        {
            x = Mathf.Clamp(x, 0f, w - 1.001f);
            z = Mathf.Clamp(z, 0f, h - 1.001f);
            var x0 = Mathf.FloorToInt(x); var z0 = Mathf.FloorToInt(z);
            var tx = x - x0; var tz = z - z0;
            var a = g[x0, z0]; var b = g[Mathf.Min(x0 + 1, w - 1), z0];
            var c = g[x0, Mathf.Min(z0 + 1, h - 1)]; var d = g[Mathf.Min(x0 + 1, w - 1), Mathf.Min(z0 + 1, h - 1)];
            return Mathf.Lerp(Mathf.Lerp(a, b, tx), Mathf.Lerp(c, d, tx), tz);
        }
    }

    public static class TerrainHeightField
    {
        public const int GridSize = 33;
        public const float ChunkSize = 256f;
        static readonly Dictionary<Vector2Int, ChunkHeightData> Cache = new();

        public static ChunkHeightData GetChunk(Vector2Int coord)
        {
            if (Cache.TryGetValue(coord, out var cached)) return cached;
            var data = BuildChunk(coord);
            if (Cache.Count > 24) Cache.Clear();
            Cache[coord] = data;
            return data;
        }

        public static float Sample(float x, float z)
        {
            var cx = Mathf.FloorToInt(x / ChunkSize);
            var cz = Mathf.FloorToInt(z / ChunkSize);
            return GetChunk(new Vector2Int(cx, cz)).SampleHeight(x, z);
        }

        static ChunkHeightData BuildChunk(Vector2Int coord)
        {
            var gs = GridSize;
            var cell = ChunkSize / (gs - 1);
            var ox = coord.x * ChunkSize;
            var oz = coord.y * ChunkSize;
            var h = new float[gs, gs];
            for (var z = 0; z < gs; z++)
            for (var x = 0; x < gs; x++)
                h[x, z] = NaturalUplift.Sample(ox + x * cell, oz + z * cell);
            var seed = coord.x * 73856093 ^ coord.y * 19349663;
            ErosionSimulator.Erode(h, gs, gs, seed);
            var flow = ErosionSimulator.FlowAccum(h, gs, gs);
            Debug.Log($"[TerrainHeightField] Built eroded chunk {coord}");
            return new ChunkHeightData
            {
                Heights = h, Flow = flow, OriginX = ox, OriginZ = oz,
                CellSize = cell, Width = gs, Height = gs
            };
        }
    }
}
