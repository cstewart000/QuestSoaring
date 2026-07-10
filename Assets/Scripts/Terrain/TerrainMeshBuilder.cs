using System.Collections.Generic;
using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class TerrainMeshBuilder
    {
        public static void BuildFlatMesh(Mesh mesh, Vector2Int coord, float chunkSize, int res)
        {
            var field = TerrainHeightField.GetChunk(coord);
            var origin = new Vector3(coord.x * chunkSize, 0f, coord.y * chunkSize);
            var verts = new List<Vector3>();
            var colors = new List<Color>();
            var tris = new List<int>();

            void AddTri(float wx0, float wz0, float wx1, float wz1, float wx2, float wz2)
            {
                var y0 = field.SampleHeight(wx0, wz0);
                var y1 = field.SampleHeight(wx1, wz1);
                var y2 = field.SampleHeight(wx2, wz2);
                var a = new Vector3(wx0, y0, wz0);
                var b = new Vector3(wx1, y1, wz1);
                var c = new Vector3(wx2, y2, wz2);
                var slope = 1f - Vector3.Dot(Vector3.Cross(b - a, c - a).normalized, Vector3.up);
                var cx = (wx0 + wx1 + wx2) / 3f;
                var cz = (wz0 + wz1 + wz2) / 3f;
                var cy = (y0 + y1 + y2) / 3f;
                var flow = field.SampleFlow(cx, cz);
                var col = BiomeMap.GetColor(cx, cz, cy, slope, flow);
                var i = verts.Count;
                verts.Add(a); verts.Add(b); verts.Add(c);
                colors.Add(col); colors.Add(col); colors.Add(col);
                tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
            }

            for (var z = 0; z < res - 1; z++)
            for (var x = 0; x < res - 1; x++)
            {
                var wx0 = origin.x + x * chunkSize / (res - 1);
                var wz0 = origin.z + z * chunkSize / (res - 1);
                var wx1 = origin.x + (x + 1) * chunkSize / (res - 1);
                var wz1 = origin.z + (z + 1) * chunkSize / (res - 1);
                AddTri(wx0, wz0, wx0, wz1, wx1, wz0);
                AddTri(wx1, wz0, wx0, wz1, wx1, wz1);
            }

            mesh.Clear();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateBounds();
            Debug.Log($"[TerrainMeshBuilder] Eroded chunk {coord}");
        }
    }
}
