using UnityEngine;

namespace VR360
{
    /// <summary>
    /// Keert de normalen en driehoeken van een SphereMesh om,
    /// zodat een 360 video aan de binnenkant van een bol zichtbaar is.
    /// Handig als alternatief voor de Skybox (bijvoorbeeld voor 3D effecten, 180 splits of belichting).
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class InvertedSphere : MonoBehaviour
    {
        [Tooltip("Grootte van de bol rondom de gebruiker (bijv. 50 meter)")]
        [SerializeField] private float radius = 50f;

        private void Awake()
        {
            InvertSphereMesh();
        }

        [ContextMenu("Inverteer Mesh")]
        public void InvertSphereMesh()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter.sharedMesh == null)
            {
                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                filter.sharedMesh = Instantiate(tempSphere.GetComponent<MeshFilter>().sharedMesh);
                DestroyImmediate(tempSphere);
            }
            else
            {
                filter.mesh = Instantiate(filter.sharedMesh);
            }

            Mesh mesh = filter.mesh;
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;

            for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
            {
                int[] triangles = mesh.GetTriangles(submesh);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i];
                    triangles[i] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, submesh);
            }

            transform.localScale = new Vector3(-radius, radius, radius);
        }
    }
}
