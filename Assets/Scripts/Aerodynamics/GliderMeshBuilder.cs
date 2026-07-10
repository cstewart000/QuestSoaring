using QuestSoaring.SceneSetup;
using UnityEngine;

namespace QuestSoaring.Aerodynamics
{
    /// <summary>Procedural low-poly fuselage + wings for MVP placeholder art.</summary>
    public static class GliderMeshBuilder
    {
        public static GameObject Build(Transform parent)
        {
            var root = new GameObject("GliderMesh");
            root.transform.SetParent(parent, false);
            AddPart(root.transform, "Fuselage", Vector3.zero, new Vector3(0.4f, 0.5f, 2.5f), new Color(0.75f, 0.75f, 0.78f));
            AddPart(root.transform, "LeftWing", new Vector3(-3f, 0f, 0.2f), new Vector3(6f, 0.08f, 1.2f), new Color(0.7f, 0.7f, 0.72f));
            AddPart(root.transform, "RightWing", new Vector3(3f, 0f, 0.2f), new Vector3(6f, 0.08f, 1.2f), new Color(0.7f, 0.7f, 0.72f));
            AddPart(root.transform, "Tail", new Vector3(0f, 0.3f, -1.3f), new Vector3(0.08f, 0.8f, 0.6f), new Color(0.68f, 0.68f, 0.7f));
            Debug.Log("[GliderMeshBuilder] Placeholder glider mesh built");
            return root;
        }

        static void AddPart(Transform parent, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            var col = go.GetComponent<Collider>();
            if (col) Object.Destroy(col);
            var rend = go.GetComponent<Renderer>();
            rend.material = RenderUtil.CreateLitMaterial(color);
        }
    }
}
