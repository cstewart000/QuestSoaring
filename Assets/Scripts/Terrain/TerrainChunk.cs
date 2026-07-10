using UnityEngine;

namespace QuestSoaring.Terrain
{
    public static class HeightNoise
    {
        public static float Sample(float x, float z, float scale, int octaves)
        {
            var h = 0f;
            var amp = 1f;
            var freq = 1f / scale;
            for (var i = 0; i < octaves; i++)
            {
                h += Mathf.PerlinNoise(x * freq, z * freq) * amp;
                amp *= 0.5f;
                freq *= 2f;
            }
            return h * 400f;
        }
    }

    public class TerrainChunk : MonoBehaviour
    {
        public Vector2Int Coord { get; private set; }
        const int Res = 16;

        public void Build(Vector2Int coord, float chunkSize, Material mat)
        {
            Coord = coord;
            var mesh = new Mesh { name = $"Chunk_{coord.x}_{coord.y}" };
            var verts = new Vector3[Res * Res];
            var tris = new int[(Res - 1) * (Res - 1) * 6];
            var origin = new Vector3(coord.x * chunkSize, 0f, coord.y * chunkSize);

            for (var z = 0; z < Res; z++)
            for (var x = 0; x < Res; x++)
            {
                var wx = origin.x + x * chunkSize / (Res - 1);
                var wz = origin.z + z * chunkSize / (Res - 1);
                verts[z * Res + x] = new Vector3(wx, HeightNoise.Sample(wx, wz, 800f, 4), wz);
            }

            var ti = 0;
            for (var z = 0; z < Res - 1; z++)
            for (var x = 0; x < Res - 1; x++)
            {
                var i = z * Res + x;
                tris[ti++] = i; tris[ti++] = i + Res; tris[ti++] = i + 1;
                tris[ti++] = i + 1; tris[ti++] = i + Res; tris[ti++] = i + Res + 1;
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = mat;
            Debug.Log($"[TerrainChunk] Built {coord}");
        }
    }
}
