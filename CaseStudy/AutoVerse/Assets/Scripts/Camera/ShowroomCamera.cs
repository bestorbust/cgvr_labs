using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// AutoVerse - ShowroomCamera
/// Smooth orbital camera controller with preset transitions, auto-rotation turntable,
/// pinch zoom, and strict UI interaction isolation with drag-threshold deadzones.
/// Correctly aligned with vehicle heading: Front View faces headlights, Rear View faces spoiler.
/// </summary>
public class ShowroomCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarSelector carSelector;

    [Header("Orbit Settings")]
    [SerializeField] private float mouseSensitivity = 0.22f;
    [SerializeField] private float touchSensitivity = 0.18f;
    [SerializeField] private float smoothing = 8f;
    [SerializeField] private float autoRotateSpeed = 15f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float pinchSensitivity = 0.015f;
    [SerializeField] private float minDistance = 2.5f;
    [SerializeField] private float maxDistance = 9.0f; // Clamped inside showroom walls

    [Header("Vertical Limits")]
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 70f;

    // Default to Front 3/4 Hero angle (car faces +Z)
    private float yaw = 215f;
    private float pitch = 18f;
    private float distance = 6.8f;

    private float targetYaw = 215f;
    private float targetPitch = 18f;
    private float targetDistance = 6.8f;

    private Vector3 targetPosition;
    private GameObject lastCar;
    private float lastAspect;
    private float framingDistance = 6.8f;
    private bool pinchAllowed;
    private int orbitFinger = -1;
    private bool autoRotate = false;

    // Drag tracking with threshold deadzone (prevents clicking buttons from moving the camera)
    private bool isMouseDownValid = false;
    private bool isActuallyDraggingMouse = false;
    private Vector2 mouseDownPos;
    private Vector2 lastMousePos;

    private bool isTouchDownValid = false;
    private bool isActuallyDraggingTouch = false;
    private Vector2 touchDownPos;
    private Vector2 lastTouchPos;

    private const float DRAG_THRESHOLD = 10.0f; // Minimum 10px intentional drag before orbiting camera

    public bool IsAutoRotating => autoRotate;
    public float CurrentYaw => yaw;
    public float CurrentPitch => pitch;
    public float CurrentDistance => distance;

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

        float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
        if (Mathf.Abs(aspect - lastAspect) > 0.01f)
        {
            lastAspect = aspect;
            FocusOnCar(lastCar);
        }
        HandleInput();

        float t = 1f - Mathf.Exp(-Mathf.Max(1, smoothing) * Time.unscaledDeltaTime);
        if (autoRotate) targetYaw += autoRotateSpeed * Time.unscaledDeltaTime;
        ClampCamera();
        yaw = Mathf.LerpAngle(yaw, targetYaw, t);
        pitch = Mathf.Lerp(pitch, targetPitch, t);
        distance = Mathf.Lerp(distance, targetDistance, t);

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
            orbitFinger = -1;
            pinchAllowed = false;
            isTouchDownValid = false;
            HandleMouse();
        }

        HandleKeyboard();
    }

    private void HandleMouse()
    {
        // 1. Mouse Button Down
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI())
            {
                isMouseDownValid = true;
                isActuallyDraggingMouse = false;
                mouseDownPos = Input.mousePosition;
                lastMousePos = Input.mousePosition;
            }
            else
            {
                isMouseDownValid = false;
                isActuallyDraggingMouse = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDownValid = false;
            isActuallyDraggingMouse = false;
        }

        // 2. Mouse Dragging with threshold check
        if (isMouseDownValid && Input.GetMouseButton(0))
        {
            Vector2 currentMouse = Input.mousePosition;

            // Only start rotating once mouse exceeds drag threshold distance
            if (!isActuallyDraggingMouse)
            {
                if (Vector2.Distance(currentMouse, mouseDownPos) >= DRAG_THRESHOLD)
                {
                    isActuallyDraggingMouse = true;
                    lastMousePos = currentMouse;
                }
            }

            if (isActuallyDraggingMouse)
            {
                Vector2 delta = currentMouse - lastMousePos;
                lastMousePos = currentMouse;

                float deltaX = delta.x * mouseSensitivity * 0.45f;
                float deltaY = delta.y * mouseSensitivity * 0.45f;

                if (Mathf.Abs(deltaX) > 0.001f || Mathf.Abs(deltaY) > 0.001f)
                {
                    targetYaw += deltaX * InputScale;
                    targetPitch -= deltaY * InputScale;
                    autoRotate = false;
                }
            }
        }

        // 3. Zoom with scrollwheel (blocked when hovering over UI)
        if (!IsPointerOverUI())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                targetDistance -= scroll * zoomSpeed;
            }
        }

        ClampCamera();
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.fingerId != orbitFinger)

            {
                orbitFinger = touch.fingerId;
                pinchAllowed = false;
                if (!IsTouchOverUI(touch.fingerId))
                {
                    isTouchDownValid = true;
                    isActuallyDraggingTouch = false;
                    touchDownPos = touch.position;
                    lastTouchPos = touch.position;
                }
                else
                {
                    isTouchDownValid = false;
                    isActuallyDraggingTouch = false;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouchDownValid = false;
                isActuallyDraggingTouch = false;
            }

            if (isTouchDownValid && touch.phase == TouchPhase.Moved)
            {
                Vector2 currentPos = touch.position;
                if (!isActuallyDraggingTouch)
                {
                    if (Vector2.Distance(currentPos, touchDownPos) >= DRAG_THRESHOLD)
                    {
                        isActuallyDraggingTouch = true;
                        lastTouchPos = currentPos;
                    }
                }

                if (isActuallyDraggingTouch)
                {
                    Vector2 delta = currentPos - lastTouchPos;
                    lastTouchPos = currentPos;

                    targetYaw += delta.x * touchSensitivity * InputScale;
                    targetPitch -= delta.y * touchSensitivity * InputScale;
                    autoRotate = false;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch first = Input.GetTouch(0);
            Touch second = Input.GetTouch(1);

            isTouchDownValid = false;
            orbitFinger = -1;
            if (first.phase == TouchPhase.Began || second.phase == TouchPhase.Began)
                pinchAllowed = !IsTouchOverUI(first.fingerId) && !IsTouchOverUI(second.fingerId);
            if (!pinchAllowed || IsTouchOverUI(first.fingerId) || IsTouchOverUI(second.fingerId)) return;

            Vector2 previousFirst = first.position - first.deltaPosition;
            Vector2 previousSecond = second.position - second.deltaPosition;

            float previousDistance = Vector2.Distance(previousFirst, previousSecond);
            float currentDistance = Vector2.Distance(first.position, second.position);
            float difference = currentDistance - previousDistance;

            targetDistance -= difference * pinchSensitivity * InputScale;
            autoRotate = false;
        }

        ClampCamera();
    }

    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ResetView();

        if (Input.GetKeyDown(KeyCode.A))
            ToggleAutoRotate();
    }

    private float InputScale => 720f / Mathf.Max(320f, Mathf.Min(Screen.width, Screen.height));

    private void ClampCamera()
    {
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        distance = Mathf.Clamp(distance, minDistance, Mathf.Max(maxDistance, framingDistance * 1.25f));
        targetDistance = Mathf.Clamp(targetDistance, minDistance, Mathf.Max(maxDistance, framingDistance * 1.25f));
    }

    private void FocusOnCar(GameObject car)
    {
        Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        targetPosition = bounds.center;

        Camera cam = GetComponent<Camera>();
        float fov = cam != null ? cam.fieldOfView : 60f;
        float aspect = cam != null ? cam.aspect : 1.777f;

        float verticalFov = fov * Mathf.Deg2Rad;
        float horizontalFov = 2f * Mathf.Atan(Mathf.Tan(verticalFov * 0.5f) * Mathf.Max(0.1f, aspect));
        float limitingHalfFov = Mathf.Min(verticalFov, horizontalFov) * 0.5f;

        float radius = bounds.extents.magnitude;
        framingDistance = Mathf.Max(minDistance, radius / Mathf.Sin(limitingHalfFov) * 1.15f);
        targetDistance = framingDistance;
    }

    private void UpdateCamera()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = targetPosition + rotation * new Vector3(0, 0, -distance);

        float t = 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);

        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, t);
        }
    }

    /// <summary>
    /// Triple-check: UI panel geometry bounds + EventSystem GraphicRaycaster + Screen margin guard zones
    /// </summary>
    private bool IsPointerOverUI()
    {
        if (AutoVerseShowroomUI.Instance != null) return AutoVerseShowroomUI.Instance.IsPointerOverUI();
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsTouchOverUI(int fingerId)
    {
        if (AutoVerseShowroomUI.Instance != null) return AutoVerseShowroomUI.Instance.IsTouchOverUI(fingerId);
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private void OnApplicationFocus(bool focused)
    {
        isMouseDownValid = isActuallyDraggingMouse = false;
        isTouchDownValid = isActuallyDraggingTouch = pinchAllowed = false;
        orbitFinger = -1;
    }

    public void SetView(float newYaw, float newPitch)
    {
        targetYaw = newYaw;
        targetPitch = newPitch;
        autoRotate = false;
        ClampCamera();
    }

    /// <summary>
    /// Front 3/4 Hero angle showing headlights, hood, and side profile
    /// </summary>
    public void SetHeroView()
    {
        targetYaw = 215f;
        targetPitch = 18f;
        targetDistance = framingDistance;
        autoRotate = false;
        ClampCamera();
    }

    /// <summary>
    /// Front View: Faces directly at the car's front bumper, grille, and headlights
    /// </summary>
    public void SetFrontView()
    {
        targetYaw = 180f;
        targetPitch = 14f;
        targetDistance = framingDistance;
        autoRotate = false;
        ClampCamera();
    }

    /// <summary>
    /// Side View: Direct profile view of the vehicle
    /// </summary>
    public void SetSideView()
    {
        targetYaw = 90f;
        targetPitch = 12f;
        targetDistance = framingDistance;
        autoRotate = false;
        ClampCamera();
    }

    /// <summary>
    /// Rear View: Faces directly at the car's rear bumper, tail lights, and spoiler
    /// </summary>
    public void SetRearView()
    {
        targetYaw = 0f;
        targetPitch = 15f;
        targetDistance = framingDistance;
        autoRotate = false;
        ClampCamera();
    }

    /// <summary>
    /// Top View: Elevated overhead view showing the hood, roof, and trunk
    /// </summary>
    public void SetTopView()
    {
        targetYaw = 215f;
        targetPitch = 68f;
        targetDistance = framingDistance * 1.08f;
        autoRotate = false;
        ClampCamera();
    }

    /// <summary>
    /// Wheel View: Focused close-up on the front-left alloy wheel
    /// </summary>
    public void SetWheelView()
    {
        targetYaw = 220f;
        targetPitch = 8f;
        targetDistance = Mathf.Clamp(distance * 0.7f, minDistance, maxDistance);
        autoRotate = false;
        ClampCamera();
    }

    public void SetAutoRotate(bool state)
    {
        autoRotate = state;
    }

    public void ResetView()
    {
        SetHeroView();
    }

    public void ToggleAutoRotate()
    {
        autoRotate = !autoRotate;
        targetYaw = yaw;
        targetPitch = pitch;
        targetDistance = distance;
    }

    public void ZoomIn()
    {
        targetDistance -= 0.75f;
        ClampCamera();
    }

    public void ZoomOut()
    {
        targetDistance += 0.75f;
        ClampCamera();
    }
}
