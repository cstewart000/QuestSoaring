# Quest Soaring — Build Plan

Based on `pdr.txt`. Unity 2022 LTS + URP, Meta Quest standalone.

## Milestones

### M1 — Project Foundation (this repo)
- [x] Git + GitHub, Unity skeleton, test framework
- [x] Cursor rule: commit each major change
- [ ] Open in Unity 2022.3 LTS, create URP 3D template, import Meta XR All-in-One

### M2 — MVP Flying (target: playable loop)
- [x] `GliderAeroModel` — lift, drag, stall heuristics
- [x] `GliderController` — applies forces to Rigidbody
- [x] `VRGliderInput` — controller tilt = yoke, twist = rudder
- [ ] Scene: glider prefab + skybox + grayscale post-processing volume
- [ ] Basic instruments: vario, altimeter (UI canvas on wrist or panel)

### M3 — World
- [x] `ProceduralTerrain` — chunked low-poly heightmap mesh
- [x] `TerrainChunk` — load/unload by player position
- [ ] Ridge lift from terrain normal vs wind
- [ ] Accent colors on horizon (teal/orange/purple gradients)

### M4 — Interaction
- [x] `InteractableLever` — physics-hand grab, haptic on snap
- [ ] Tow release, air brakes, landing gear levers in cockpit
- [ ] XR Interaction Toolkit + XR Hands wiring

### M5 — Weather & Progression
- [x] `ThermalField` — column lift zones
- [ ] Wind layers, turbulence noise
- [ ] Modes: free flight, thermal hunt, cross-country, training

### M6 — Polish
- Multiple glider profiles, sound, menus, save/settings

## Architecture

```
Assets/Scripts/
  Aerodynamics/   Pure math + MonoBehaviour controller
  Terrain/        Chunk generation & streaming
  Weather/        Thermals, wind
  VR/             Input mapping, levers
  Tests/          EditMode unit tests (NUnit)
```

## Performance (Quest 3 @ 72 FPS)
- Heuristic physics only (no CFD)
- Low-poly meshes, limited draw calls per chunk
- Fixed timestep physics: 50 Hz

## Testing

Run in Unity: **Window → General → Test Runner → EditMode → Run All**

| Test assembly | Covers |
|---------------|--------|
| `QuestSoaring.Tests.EditMode` | Aero math, thermals, terrain height |

## Manual VR test checklist
1. Launch on Quest, verify 72 FPS in empty scene
2. Pitch/roll with controller tilt; yaw with twist
3. Stall below ~12 m/s indicated; recover with nose down
4. Enter thermal column; vario shows climb
5. Grab air brake lever; drag increases
