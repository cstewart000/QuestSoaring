using UnityEngine;

namespace QuestSoaring.SceneSetup
{
    /// <summary>Builds playable MVP scene at runtime when Main.unity loads.</summary>
    public class SceneBootstrap : MonoBehaviour
    {
        [SerializeField] bool _buildOnAwake = true;
        static bool _built;

        void Awake()
        {
            if (!_buildOnAwake || _built) return;
            _built = true;
            Build();
        }

        public void Build()
        {
            if (GameObject.Find("QuestSoaringWorld") != null)
            {
                Debug.Log("[SceneBootstrap] World already built, skipping");
                return;
            }
            _built = true;
            Debug.Log("[SceneBootstrap] Building Main scene...");
            var root = new GameObject("QuestSoaringWorld").transform;
            WorldFactory.CreateSun(root);
            GrayscalePostSetup.Apply(root);
            var mat = WorldFactory.CreateTerrainMaterial();
            var thermals = WorldFactory.CreateThermals(root);
            var rig = GliderFactory.Create(root, thermals);
            WorldFactory.CreateTerrain(root, rig.Root, mat);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.72f, 0.76f, 0.82f);
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.0008f;
            Debug.Log("[SceneBootstrap] Ready — W/S pitch A/D roll Q/E rudder");
        }
    }
}
