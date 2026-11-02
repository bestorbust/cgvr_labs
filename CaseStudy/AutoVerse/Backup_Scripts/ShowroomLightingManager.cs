
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShowroomLightingManager : MonoBehaviour
{
    public enum LightingStyle
    {
        LuxuryStudio,
        MidnightStudio,
        BrightGallery,
        NeonGarage
    }

    [Header("Scene References")]
    [SerializeField] private CarSelector carSelector;
    [SerializeField] private Transform originalLightingRoot;

    [Header("Lighting")]
    [SerializeField] private LightingStyle startingStyle =
        LightingStyle.LuxuryStudio;

    [Range(0.1f, 2f)]
    [SerializeField] private float masterBrightness = 1f;

    [SerializeField] private bool disableTemplateLights = true;

    [Header("Controls")]
    [SerializeField] private bool enableKeyboardShortcuts = true;

    private Light keyLight;
    private Light fillLight;
    private Light rimLight;
    private Light accentLight;

    private Camera mainCamera;
    private Transform rigRoot;
    private LightingStyle currentStyle;

    private readonly Dictionary<Light, bool> previousLightStates =
        new Dictionary<Light, bool>();

    private AmbientMode previousAmbientMode;
    private Color previousAmbientColor;
    private float previousAmbientIntensity;

    private CameraClearFlags previousClearFlags;
    private Color previousBackgroundColor;

    private void Awake()
    {
        if (carSelector == null)
            carSelector = FindAnyObjectByType<CarSelector>();

        mainCamera = Camera.main;

        previousAmbientMode = RenderSettings.ambientMode;
        previousAmbientColor = RenderSettings.ambientLight;
        previousAmbientIntensity = RenderSettings.ambientIntensity;

        if (mainCamera != null)
        {
            previousClearFlags = mainCamera.clearFlags;
            previousBackgroundColor = mainCamera.backgroundColor;
        }

        if (disableTemplateLights && originalLightingRoot != null)
        {
            Light[] oldLights =
                originalLightingRoot.GetComponentsInChildren<Light>(true);

            foreach (Light light in oldLights)
            {
                previousLightStates[light] = light.enabled;
                light.enabled = false;
            }
        }

        GameObject rig = new GameObject("AutoVerse Lighting Rig");
        rig.transform.SetParent(transform, false);
        rigRoot = rig.transform;

        keyLight = CreateSpotlight("Key Light", 115f);
        fillLight = CreateSpotlight("Fill Light", 120f);
        rimLight = CreateSpotlight("Rim Light", 110f);
        accentLight = CreateSpotlight("Accent Light", 115f);
    }

    private Light CreateSpotlight(string name, float angle)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(rigRoot, false);

        Light light = obj.AddComponent<Light>();

        light.type = LightType.Spot;
        light.spotAngle = angle;
        light.range = 22f;
        light.shadows = LightShadows.None;
        light.intensity = 1f;

        return light;
    }

    private void Start()
    {
        SetLightingStyle((int)startingStyle);
    }

    private void Update()
    {
        UpdateLightPositions();

        if (!enableKeyboardShortcuts)
            return;

        if (Input.GetKeyDown(KeyCode.F1))
            SetLightingStyle(0);

        if (Input.GetKeyDown(KeyCode.F2))
            SetLightingStyle(1);

        if (Input.GetKeyDown(KeyCode.F3))
            SetLightingStyle(2);

        if (Input.GetKeyDown(KeyCode.F4))
            SetLightingStyle(3);

        if (Input.GetKeyDown(KeyCode.Equals) ||
            Input.GetKeyDown(KeyCode.KeypadPlus))
            AdjustBrightness(0.1f);

        if (Input.GetKeyDown(KeyCode.Minus) ||
            Input.GetKeyDown(KeyCode.KeypadMinus))
            AdjustBrightness(-0.1f);
    }

    private void UpdateLightPositions()
    {
        Vector3 center = Vector3.zero;

        if (carSelector != null && carSelector.CurrentCar != null)
        {
            Renderer[] renderers =
                carSelector.CurrentCar.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;

                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                center = bounds.center;
            }
        }

        // Car models are centered near the showroom origin.
        // Keep the lighting rig aligned with the active vehicle.
        PositionLight(
            keyLight,
            center + new Vector3(-3.5f, 5f, -3.5f),
            center);

        PositionLight(
            fillLight,
            center + new Vector3(3.5f, 3.5f, -2.5f),
            center);

        PositionLight(
            rimLight,
            center + new Vector3(2.5f, 4.5f, 3.5f),
            center);

        PositionLight(
            accentLight,
            center + new Vector3(-3f, 3f, 3.5f),
            center);
    }

    private void PositionLight(
        Light light,
        Vector3 position,
        Vector3 target)
    {
        if (light == null)
            return;

        light.transform.position = position;
        light.transform.LookAt(target);
    }

    public void SetLightingStyle(int index)
    {
        if (index < 0 || index > 3)
            return;

        currentStyle = (LightingStyle)index;
        ApplyCurrentStyle();

        Debug.Log("AutoVerse Lighting: " + currentStyle);
    }

    private void ApplyCurrentStyle()
    {
        switch (currentStyle)
        {
            case LightingStyle.LuxuryStudio:
                ApplyStyle(
                    new Color(1f, 0.94f, 0.84f),
                    new Color(0.78f, 0.88f, 1f),
                    Color.white,
                    new Color(1f, 0.72f, 0.40f),
                    7f, 4.5f, 6f, 1.5f,
                    new Color(0.18f, 0.19f, 0.22f),
                    new Color(0.045f, 0.05f, 0.065f));
                break;

            case LightingStyle.MidnightStudio:
                ApplyStyle(
                    new Color(0.82f, 0.90f, 1f),
                    new Color(0.30f, 0.48f, 1f),
                    new Color(0.35f, 0.72f, 1f),
                    new Color(1f, 0.42f, 0.25f),
                    3.5f, 2f, 8f, 2f,
                    new Color(0.07f, 0.09f, 0.15f),
                    new Color(0.008f, 0.012f, 0.025f));
                break;

            case LightingStyle.BrightGallery:
                ApplyStyle(
                    Color.white,
                    new Color(0.94f, 0.97f, 1f),
                    Color.white,
                    new Color(0.88f, 0.93f, 1f),
                    9f, 7f, 5f, 2f,
                    new Color(0.28f, 0.29f, 0.30f),
                    new Color(0.18f, 0.19f, 0.21f));
                break;

            case LightingStyle.NeonGarage:
                ApplyStyle(
                    new Color(1f, 0.95f, 0.90f),
                    new Color(0.12f, 0.40f, 1f),
                    new Color(1f, 0.12f, 0.65f),
                    new Color(0.30f, 0.10f, 1f),
                    5f, 3f, 8f, 4f,
                    new Color(0.10f, 0.10f, 0.17f),
                    new Color(0.025f, 0.012f, 0.055f));
                break;
        }
    }

    private void ApplyStyle(
        Color keyColor,
        Color fillColor,
        Color rimColor,
        Color accentColor,
        float keyIntensity,
        float fillIntensity,
        float rimIntensity,
        float accentIntensity,
        Color ambientColor,
        Color backgroundColor)
    {
        ConfigureLight(keyLight, keyColor, keyIntensity);
        ConfigureLight(fillLight, fillColor, fillIntensity);
        ConfigureLight(rimLight, rimColor, rimIntensity);
        ConfigureLight(accentLight, accentColor, accentIntensity);

        // Flat ambient lighting keeps dark car surfaces visible.
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = 1f;

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = backgroundColor;
        }
    }

    private void ConfigureLight(
        Light light,
        Color color,
        float intensity)
    {
        if (light == null)
            return;

        light.color = color;
        light.intensity = intensity * masterBrightness;
    }

    public void SetBrightness(float value)
    {
        masterBrightness = Mathf.Clamp(value, 0.1f, 2f);
        ApplyCurrentStyle();
    }

    public void AdjustBrightness(float amount)
    {
        SetBrightness(masterBrightness + amount);
        Debug.Log("Lighting brightness: " + masterBrightness);
    }

    private void OnDestroy()
    {
        foreach (var entry in previousLightStates)
        {
            if (entry.Key != null)
                entry.Key.enabled = entry.Value;
        }

        RenderSettings.ambientMode = previousAmbientMode;
        RenderSettings.ambientLight = previousAmbientColor;
        RenderSettings.ambientIntensity = previousAmbientIntensity;

        if (mainCamera != null)
        {
            mainCamera.clearFlags = previousClearFlags;
            mainCamera.backgroundColor = previousBackgroundColor;
        }
    }
}
