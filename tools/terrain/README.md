# Terrain tools (no Unity required)

## Quick start (recommended)

```bash
python3 tools/terrain/serve.py
```

Opens http://127.0.0.1:8765/preview_standalone.html automatically.

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Blank page / module errors | **Don't double-click the HTML file.** Must use `serve.py` |
| `Address already in use` | `serve.py` tries 8766, 8767, 9080 automatically |
| "Load failed" / Three.js | Need internet for CDN (unpkg.com) |
| Still broken | Try: `cd tools/terrain && python3 serve.py` |

## Export (no server needed)

```bash
python3 tools/terrain/export_terrain.py tools/terrain/export
```

Opens `terrain_c0_0.obj` in Blender if preview won't run.

## Legacy multi-file preview

`preview.html` + `chunk_mesh.js` — same server, use `/preview.html` instead.
