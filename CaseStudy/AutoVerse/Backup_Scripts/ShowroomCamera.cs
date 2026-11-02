
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowroomCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarSelector carSelector;

    [Header("Orbit Settings")]
    [SerializeField] private float mouseSensitivity = 0.25f;
    [SerializeField] private float touchSensitivity = 0.18f;
    [SerializeField] private float smoothing = 8f;
    [SerializeField] private float autoRotateSpeed = 15f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float pinchSensitivity = 0.015f;
    [SerializeField] private float minDistance = 2.5f;
    [SerializeField] private float maxDistance = 15f;

    [Header("Vertical Limits")]
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 80f;

    private float yaw = 35f;
    private float pitch = 20f;
    private float distance = 7f;

    private Vector3 targetPosition;
    private GameObject lastCar;
    private bool autoRotate;

    private void Start()
    {
        if (carSelector == null)
            carSelector = FindAnyObjectByType<CarSelector>();

        ResetView();
    }

    private void LateUpdate()
    {
        if (carSelector == null || carSelector.CurrentCar == null)
            return;

        if (lastCar != carSelector.CurrentCar)
        {
            lastCar = carSelector.CurrentCar;
            FocusOnCar(lastCar);
        }

        HandleInput();

        if (autoRotate)
            yaw += autoRotateSpeed * Time.deltaTime;

        UpdateCamera();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }

        HandleKeyboard();
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButton(0) && !IsPointerOverUI())
        {
            yaw += Input.GetAxis("Mouse X")
                * mouseSensitivity * 10f;

            pitch -= Input.GetAxis("Mouse Y")
                * mouseSensitivity * 10f;

            autoRotate = false;
        }

        if (!IsPointerOverUI())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            distance -= scroll * zoomSpeed;
        }

        ClampCamera();
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (IsTouchOverUI(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Moved)
            {
                yaw += touch.deltaPosition.x * touchSensitivity;
                pitch -= touch.deltaPosition.y * touchSensitivity;

                autoRotate = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch first = Input.GetTouch(0);
            Touch second = Input.GetTouch(1);

            if (IsTouchOverUI(first.fingerId) ||
                IsTouchOverUI(second.fingerId))
                return;

            Vector2 previousFirst =
                first.position - first.deltaPosition;

            Vector2 previousSecond =
                second.position - second.deltaPosition;

            float previousDistance =
                Vector2.Distance(previousFirst, previousSecond);

            float currentDistance =
                Vector2.Distance(first.position, second.position);

            float difference =
                currentDistance - previousDistance;

            distance -= difference * pinchSensitivity;
        }

        ClampCamera();
    }

    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.F))
            SetView(0f, 15f);

        if (Input.GetKeyDown(KeyCode.B))
            SetView(180f, 15f);

        if (Input.GetKeyDown(KeyCode.L))
            SetView(-90f, 15f);

        if (Input.GetKeyDown(KeyCode.R))
            SetView(90f, 15f);

        if (Input.GetKeyDown(KeyCode.T))
            SetView(35f, 80f);

        if (Input.GetKeyDown(KeyCode.Space))
            ResetView();

        if (Input.GetKeyDown(KeyCode.A))
            ToggleAutoRotate();
    }

    private void ClampCamera()
    {
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        distance = Mathf.Clamp(
            distance, minDistance, maxDistance);
    }

    private void FocusOnCar(GameObject car)
    {
        Renderer[] renderers =
            car.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        targetPosition = bounds.center;

        Camera cam = GetComponent<Camera>();

        float fov = cam != null ? cam.fieldOfView : 60f;
        float aspect = cam != null ? cam.aspect : 1.777f;

        float verticalFov = fov * Mathf.Deg2Rad;
        float horizontalFov = 2f * Mathf.Atan(
            Mathf.Tan(verticalFov * 0.5f) *
            Mathf.Max(0.1f, aspect));

        float limitingHalfFov = Mathf.Min(
            verticalFov, horizontalFov) * 0.5f;

        float radius = bounds.extents.magnitude;

        distance = Mathf.Clamp(
            radius / Mathf.Sin(limitingHalfFov) * 1.25f,
            minDistance,
            maxDistance);
    }

    private void UpdateCamera()
    {
        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredPosition =
            targetPosition +
            rotation * new Vector3(0, 0, -distance);

        float t = 1f - Mathf.Exp(
            -smoothing * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position, desiredPosition, t);

        Vector3 direction =
            targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation, desiredRotation, t);
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsTouchOverUI(int fingerId)
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    public void SetView(float newYaw, float newPitch)
    {
        yaw = newYaw;
        pitch = newPitch;
        autoRotate = false;
        ClampCamera();
    }

    public void ResetView()
    {
        SetView(35f, 20f);
    }

    public void ToggleAutoRotate()
    {
        autoRotate = !autoRotate;
    }

    public void ZoomIn()
    {
        distance -= 0.75f;
        ClampCamera();
    }

    public void ZoomOut()
    {
        distance += 0.75f;
        ClampCamera();
    }
}
