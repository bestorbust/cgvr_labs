
using UnityEngine;

public class CarSelector : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform vehiclesRoot;

    [Header("Display Settings")]
    [SerializeField] private float platformTopY = 0f;
    [SerializeField] private int startingIndex = 0;

    private readonly string[] carNames =
    {
        "car_sports",
        "car_sedan",
        "car_hatchback",
        "car_suv",
        "race_car",
        "car_police",
        "car_taxi",
        "golf_cart"
    };

    private GameObject[] cars;
    private Vector3[] originalPositions;
    private int currentIndex;

    public int CurrentIndex => currentIndex;

    public GameObject CurrentCar =>
        cars != null &&
        currentIndex >= 0 &&
        currentIndex < cars.Length
            ? cars[currentIndex]
            : null;

    private void Awake()
    {
        if (vehiclesRoot == null)
        {
            GameObject root = GameObject.Find("Vehicles");

            if (root != null)
                vehiclesRoot = root.transform;
        }

        if (vehiclesRoot == null)
        {
            Debug.LogError("CarSelector: Vehicles parent not found!");
            enabled = false;
            return;
        }

        cars = new GameObject[carNames.Length];
        originalPositions = new Vector3[carNames.Length];

        for (int i = 0; i < carNames.Length; i++)
        {
            Transform child = vehiclesRoot.Find(carNames[i]);

            if (child == null)
            {
                Debug.LogError("Missing vehicle: " + carNames[i]);
                continue;
            }

            cars[i] = child.gameObject;
            originalPositions[i] = child.localPosition;
        }
    }

    private void Start()
    {
        ShowCar(startingIndex);
    }

    private void Update()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);

            if (Input.GetKeyDown(key))
                ShowCar(i);
        }
    }

    public void ShowCar(int index)
    {
        if (cars == null || index < 0 || index >= cars.Length)
            return;

        if (cars[index] == null)
            return;

        currentIndex = index;

        // Disable all vehicles first
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i] != null)
                cars[i].SetActive(false);
        }

        // Activate only selected vehicle
        GameObject selected = cars[currentIndex];
        selected.SetActive(true);

        // Restore original position before calculating bounds
        selected.transform.localPosition =
            originalPositions[currentIndex];

        CenterAndGround(selected);

        Debug.Log(
            "ACTIVE VEHICLE: " + selected.name +
            " | Active: " + selected.activeInHierarchy
        );
    }

    private void CenterAndGround(GameObject car)
    {
        Renderer[] renderers =
            car.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        Vector3 offset = new Vector3(
            -bounds.center.x,
            platformTopY - bounds.min.y,
            -bounds.center.z
        );

        car.transform.position += offset;
    }

    public void NextCar()
    {
        ShowCar((currentIndex + 1) % cars.Length);
    }

    public void PreviousCar()
    {
        ShowCar((currentIndex - 1 + cars.Length) % cars.Length);
    }
}
