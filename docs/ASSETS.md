# Quest Soaring — Asset Gallery

Open **`docs/asset_gallery.html`** in a browser to browse all visual references.

## Art direction (from PDR)

| Element | Spec |
|---------|------|
| Terrain / glider | Low-poly, **grayscale** dominant |
| Sky accents | Soft **teal**, **orange**, **purple** |
| Instruments | Clean, readable, dark panel |
| Mood | Peaceful, meditative, exhilarating |

## Palette

| Swatch | Hex | Use |
|--------|-----|-----|
| Terrain gray | `#8C8C94` | Ground, wings |
| Panel dark | `#1A1A1F` | Instrument background |
| Text light | `#E6EBE0` | HUD labels |
| Accent teal | `#5BA3A8` | Positive vario, horizon |
| Accent orange | `#D4895C` | Sunset glow |
| Accent purple | `#8B7FA8` | Upper sky |

## Files

| Asset | Path |
|-------|------|
| Wireframe cockpit concept | `concept_art/image.jpg` |
| Low-poly VR cockpit concept | `concept_art/content.jpeg` |
| Generated soaring vista | `docs/generated/quest-soaring-vista.png` |
| Generated instrument HUD | `docs/generated/quest-soaring-instruments.png` |
| Terrain heightmap sample | `tools/terrain/export/heightmap_c0_0.png` |

## Terrain preview (outside Unity)

```bash
cd tools/terrain && python3 -m http.server 8765
# → http://localhost:8765/preview.html
```

See [tools/terrain/README.md](../tools/terrain/README.md).

## Code assets (runtime, no mesh files yet)

| Asset | Built by |
|-------|----------|
| Glider mesh | `GliderMeshBuilder` — cubes, URP lit |
| Terrain chunks | `TerrainChunk` — procedural heightmap |
| Instrument panel | `GliderFactory` — world-space canvas |
| Control hints | `ControlHintsUI` — screen overlay |

## After Unity opens

1. Import generated PNGs as **Sprites** if using on UI skybox quads
2. Replace placeholder cubes with Blender low-poly glider FBX
3. URP Lit materials already match grayscale direction
