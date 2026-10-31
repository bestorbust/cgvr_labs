
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleMesh : MonoBehaviour
{
    void Awake()
    {
        Mesh mesh = new Mesh();
        mesh.name = "RGB Triangle";

        mesh.vertices = new Vector3[]
        {
            new Vector3(0f, 1.5f, 0f),     // Top - Red
            new Vector3(-1.5f, -1f, 0f),   // Bottom Left - Green
            new Vector3(1.5f, -1f, 0f)     // Bottom Right - Blue
        };

        mesh.triangles = new int[]
        {
            0, 1, 2
        };

        mesh.colors = new Color[]
        {
            Color.red,
            Color.green,
            Color.blue
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
