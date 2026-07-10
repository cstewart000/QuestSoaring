# First Run Checklist (before Play)

Do these **once** when opening the project in Unity 2022.3 LTS.

## 1. Let packages resolve
- Open project in Unity Hub → **2022.3.62f3** (or newer 2022.3 LTS)
- Wait for Package Manager to finish (Meta XR, URP, XR Hands)
- Console should have **no compile errors**

## 2. Run preflight (30 seconds)
Menu: **Quest Soaring → Preflight Check**

Fix anything flagged in the Console.

## 3. Setup URP (required for materials + grayscale)
Menu: **Quest Soaring → Setup URP Pipeline**

Without this, terrain/glider may render **pink** and post-processing is skipped.

## 4. Run tests
**Window → General → Test Runner → EditMode → Run All**  
Expect **10 passed**.

## 5. Play in editor
1. Open `Assets/Scenes/Main.unity`
2. Press **Play**
3. Controls: **W/S** pitch, **A/D** roll, **Q/E** rudder
4. Watch Console for `[SceneBootstrap] Ready`

You should spawn at ~600 m with terrain below and instruments in view.

## 6. Quest VR (optional, before device build)
Menu: **Quest Soaring → Setup Main Scene**

This parents **OVRCameraRig** to the glider and saves the scene.

Then:
- **Edit → Project Settings → XR Plug-in Management → Android** → enable OpenXR + Oculus
- Switch platform to Android, build to Quest

---

## Known first-open behaviour
| Symptom | Fix |
|---------|-----|
| Pink materials | Setup URP Pipeline (step 3) |
| Empty scene on Play | Ensure `SceneBootstrap` is in Main.unity |
| No world on second Play | Fixed — bootstrap rebuilds each Play |
| Package resolve slow | Normal; Meta SDK is large |
| Input System conflict | Use **Edit → Project Settings → Player → Active Input Handling → Both** if prompted |

## Not done yet (post-MVP)
- Ridge lift, wind layers
- XRI hand-grab on levers
- Sound, menus, save system
