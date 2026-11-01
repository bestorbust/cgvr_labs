
using UnityEngine;

public class CrystalCollectible : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up,
            rotationSpeed * Time.deltaTime);

        transform.position = startPosition +
            Vector3.up * Mathf.Sin(Time.time * floatSpeed)
            * floatHeight;
    }

    
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectCrystal();
        }

        Destroy(gameObject);
    }
}

}
