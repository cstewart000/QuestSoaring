using QuestSoaring.Terrain;
using QuestSoaring.Weather;
using UnityEngine;

namespace QuestSoaring.SceneSetup
{
    public static class WorldFactory
    {
        public static Material CreateTerrainMaterial()
        {
            return RenderUtil.CreateLitMaterial(new Color(0.55f, 0.55f, 0.58f));
        }

        public static ThermalField CreateThermals(Transform parent)
        {
            var go = new GameObject("ThermalField");
            go.transform.SetParent(parent);
            var field = go.AddComponent<ThermalField>();
            field.SetColumns(new[]
            {
                new ThermalColumn { CenterXZ = new Vector2(200f, 200f), Radius = 80f, CoreClimbMs = 2.5f },
                new ThermalColumn { CenterXZ = new Vector2(-150f, 400f), Radius = 60f, CoreClimbMs = 2f },
                new ThermalColumn { CenterXZ = new Vector2(0f, 800f), Radius = 100f, CoreClimbMs = 3f }
            });
            Debug.Log("[WorldFactory] Thermals placed");
            return field;
        }

        public static ProceduralTerrain CreateTerrain(Transform parent, Transform follow, Material mat)
        {
            var go = new GameObject("ProceduralTerrain");
            go.transform.SetParent(parent);
            var terrain = go.AddComponent<ProceduralTerrain>();
            terrain.Init(follow, mat);
            Debug.Log("[WorldFactory] Terrain streaming ready");
            return terrain;
        }

        public static void CreateSun(Transform parent)
        {
            var go = new GameObject("Sun");
            go.transform.SetParent(parent);
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.color = new Color(1f, 0.96f, 0.9f);
        }
    }
}
