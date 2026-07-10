# Terrain tools (no Unity required)

## Quick start

```bash
python3 tools/terrain/serve.py
```

First load runs **erosion simulation** per chunk (~2–5 s) — then cached.

## How terrain is built

1. **Tectonic uplift** — continental scale + oriented mountain ridges
2. **Hydraulic erosion** — thousands of water droplets carve valleys
3. **Thermal erosion** — talus slides material down steep slopes
4. **Flow accumulation** — drainage network forms natural rivers
5. **Biome coloring** — rivers follow flow, forests in mid elevations

Same pipeline in Unity (`TerrainHeightField`) and browser preview.

## Export

```bash
python3 tools/terrain/export_terrain.py tools/terrain/export
```

Note: export uses uplift only (fast); preview/Unity use full erosion.
