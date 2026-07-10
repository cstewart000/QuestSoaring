using UnityEngine;

namespace QuestSoaring.Terrain
{
    public class ProceduralTerrain : MonoBehaviour
    {
        [SerializeField] Transform _followTarget;
        [SerializeField] Material _chunkMaterial;
        [SerializeField] float _chunkSize = 256f;
        [SerializeField] int _viewRadius = 2;

        readonly System.Collections.Generic.Dictionary<Vector2Int, TerrainChunk> _chunks = new();

        void Update()
        {
            if (_followTarget == null) return;
            SyncChunks();
        }

        void SyncChunks()
        {
            var cx = Mathf.FloorToInt(_followTarget.position.x / _chunkSize);
            var cz = Mathf.FloorToInt(_followTarget.position.z / _chunkSize);
            var needed = new System.Collections.Generic.HashSet<Vector2Int>();

            for (var dz = -_viewRadius; dz <= _viewRadius; dz++)
            for (var dx = -_viewRadius; dx <= _viewRadius; dx++)
            {
                var coord = new Vector2Int(cx + dx, cz + dz);
                needed.Add(coord);
                if (_chunks.ContainsKey(coord)) continue;
                SpawnChunk(coord);
            }

            var toRemove = new System.Collections.Generic.List<Vector2Int>();
            foreach (var kv in _chunks)
                if (!needed.Contains(kv.Key)) toRemove.Add(kv.Key);
            foreach (var c in toRemove) RemoveChunk(c);
        }

        void SpawnChunk(Vector2Int coord)
        {
            var go = new GameObject($"Terrain_{coord.x}_{coord.y}");
            go.transform.SetParent(transform);
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            var chunk = go.AddComponent<TerrainChunk>();
            chunk.Build(coord, _chunkSize, _chunkMaterial);
            _chunks[coord] = chunk;
            Debug.Log($"[ProceduralTerrain] Spawned chunk {coord}");
        }

        void RemoveChunk(Vector2Int coord)
        {
            if (!_chunks.TryGetValue(coord, out var chunk)) return;
            Destroy(chunk.gameObject);
            _chunks.Remove(coord);
            Debug.Log($"[ProceduralTerrain] Removed chunk {coord}");
        }
    }
}
