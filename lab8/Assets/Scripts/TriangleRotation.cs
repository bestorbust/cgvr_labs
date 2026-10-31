
using UnityEngine;

public class TriangleRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 45f;
    public bool autoRotate = true;

    void Update()
    {
        if (autoRotate)
        {
            transform.Rotate(
                0f,
                0f,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            autoRotate = !autoRotate;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
