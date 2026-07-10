# Terrain tools (no Unity required)

Preview and export the same procedural terrain used in-game.

## Browser 3D preview

```bash
cd tools/terrain
python3 -m http.server 8765
```

Open http://localhost:8765/preview.html

- Low-poly chunked terrain (16×16 verts per chunk, 256 m tiles)
- Same octave Perlin params as `HeightNoise.cs` (scale 800, 4 octaves, ×400)
- **Drag** orbit · **Scroll** zoom · **WASD** pan (streams chunks like Unity)

## Export heightmap + OBJ

```bash
python3 tools/terrain/export_terrain.py tools/terrain/export
```

Outputs:
- `heightmap_c0_0.png` — grayscale height preview
- `terrain_c0_0.obj` — mesh for Blender / MeshLab

## Files

| File | Role |
|------|------|
| `height_noise.js` | Height sampling (JS port) |
| `chunk_mesh.js` | Chunk mesh builder + streaming coords |
| `preview.html` | Three.js viewer |
| `export_terrain.py` | PNG + OBJ export |

**Note:** Perlin RNG seed differs from Unity's internal permutation, so shapes will be *similar* but not pixel-identical to in-engine terrain. Algorithm structure matches.
