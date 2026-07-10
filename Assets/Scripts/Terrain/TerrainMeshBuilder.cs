using System.Collections.Generic;
using UnityEngine;

namespace QuestSoaring.Terrain
{
    /// <summary>Flat-shaded low-poly mesh with grayscale vertex colors.</summary>
    public static class TerrainMeshBuilder
    {
        public static void BuildFlatMesh(Mesh mesh, Vector2Int coord, float chunkSize, int res)
        {
            var origin = new Vector3(coord.x * chunkSize, 0f, coord.y * chunkSize);
            var heights = new float[res * res];
            for (var z = 0; z < res; z++)
            for (var x = 0; x < res; x++)
            {
                var wx = origin.x + x * chunkSize / (res - 1);
                var wz = origin.z + z * chunkSize / (res - 1);
                heights[z * res + x] = HeightNoise.Sample(wx, wz);
            }

            var verts = new List<Vector3>();
            var colors = new List<Color>();
            var tris = new List<int>();

            void AddTri(int x0, int z0, int x1, int z1, int x2, int z2)
            {
                Vector3 Pos(int x, int z)
                {
                    var wx = origin.x + x * chunkSize / (res - 1);
                    var wz = origin.z + z * chunkSize / (res - 1);
                    return new Vector3(wx, heights[z * res + x], wz);
                }
                var a = Pos(x0, z0);
                var b = Pos(x1, z1);
                var c = Pos(x2, z2);
                var slope = 1f - Vector3.Dot(Vector3.Cross(b - a, c - a).normalized, Vector3.up);
                var avgY = (a.y + b.y + c.y) / 3f;
                var col = TerrainStyle.HeightToGray(avgY, slope);
                var i = verts.Count;
                verts.Add(a); verts.Add(b); verts.Add(c);
                colors.Add(col); colors.Add(col); colors.Add(col);
                tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
            }

            for (var z = 0; z < res - 1; z++)
            for (var x = 0; x < res - 1; x++)
            {
                AddTri(x, z, x, z + 1, x + 1, z);
                AddTri(x + 1, z, x, z + 1, x + 1, z + 1);
            }

            mesh.Clear();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateBounds();
            Debug.Log($"[TerrainMeshBuilder] Flat chunk {coord} — {tris.Count / 3} tris");
        }
    }
}
