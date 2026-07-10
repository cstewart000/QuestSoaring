using UnityEngine;

namespace QuestSoaring.Terrain
{
    public class TerrainChunk : MonoBehaviour
    {
        public Vector2Int Coord { get; private set; }
        const int Res = 16;

        public void Build(Vector2Int coord, float chunkSize, Material mat)
        {
            Coord = coord;
            var mesh = new Mesh { name = $"Chunk_{coord.x}_{coord.y}" };
            TerrainMeshBuilder.BuildFlatMesh(mesh, coord, chunkSize, Res);
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
    }
}
