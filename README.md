# Quest Soaring

VR glider soaring simulator for Meta Quest. Low-poly world, heuristic Condor-style physics, natural VR yoke controls.

See [docs/BUILD_PLAN.md](docs/BUILD_PLAN.md) for milestones and architecture.

## Requirements

- Unity **2022.3 LTS** with **URP**
- Meta XR All-in-One SDK (Package Manager → Meta XR SDK)
- XR Interaction Toolkit, XR Hands
- Meta Quest 3 / 3S for device testing

## Quick Start

1. Clone this repo.
2. Open folder in Unity Hub → **2022.3 LTS** → 3D (URP) if ProjectSettings need regeneration.
3. Install Meta XR SDK via [Meta developer docs](https://developer.oculus.com/documentation/unity/unity-package-manager/).
4. Open `Assets/Scenes/Main.unity` (create scene after first open if missing).
5. Add `GliderController`, `VRGliderInput`, `Rigidbody` to glider root; tag player transform for terrain.

## Tests

**Window → General → Test Runner → EditMode → Run All**

Tests live in `Assets/Scripts/Tests/EditMode/` and cover aerodynamics and thermal math without a headset.

## Controls (VR)

| Input | Action |
|-------|--------|
| Controller pitch/roll | Yoke (pitch & roll) |
| Controller twist / thumbstick X | Rudder |
| Grab levers | Air brakes, tow release, gear |

## License

Private — all rights reserved unless otherwise noted.
