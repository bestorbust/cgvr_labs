using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// AutoVerse - ShowroomLightingManager
/// Controls studio key, fill, rim, and softbox lights across 4 luxury automotive presets.
/// Automatically eliminates legacy tripod meshes, floor spoke sticks, and template spotlight fixtures.
/// Guarantees bright, high-contrast, professional 360-degree vehicle illumination.
/// </summary>
public class ShowroomLightingManager : MonoBehaviour
{
    public static ShowroomLightingManager Instance { get; private set; }

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

    [Header("Lighting Settings")]
    [SerializeField] private LightingStyle startingStyle = LightingStyle.LuxuryStudio;
    [Range(0.5f, 3.0f)]
    [SerializeField] private float masterBrightness = 1.25f;
    [SerializeField] private bool disableTemplateLights = true;

    [Header("Controls")]
    [SerializeField] private bool enableKeyboardShortcuts = true;

    private Light mainStudioKeyLight;
    private Light fillStudioLight;
    private Light overheadSoftboxLight;
    private Light rimAccentLight;
    private Light frontAccentLight;

    private Camera mainCamera;
    private Transform rigRoot;
    private LightingStyle currentStyle;

    public LightingStyle CurrentStyle => currentStyle;
    public int CurrentStyleIndex => (int)currentStyle;
    public string CurrentStyleName
    {
        get
        {
            switch (currentStyle)
            {
                case LightingStyle.LuxuryStudio: return "Luxury Studio";
                case LightingStyle.MidnightStudio: return "Midnight Studio";
                case LightingStyle.BrightGallery: return "Bright Gallery";
                case LightingStyle.NeonGarage: return "Neon Garage";
                default: return "Studio";
            }
        }
    }

    private readonly Dictionary<Light, bool> previousLightStates = new Dictionary<Light, bool>();
    private AmbientMode previousAmbientMode;
    private Color previousAmbientColor;
    private float previousAmbientIntensity;
    private CameraClearFlags previousClearFlags;
    private Color previousBackgroundColor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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

        CleanLegacyFixtures();
        BuildStudioLightingRig();
    }

    /// <summary>
    /// Bulletproof elimination of legacy tripod meshes, radial spoke lights, and indoor spotlight fixtures.
    /// Disables MeshRenderers on any objects named Tripod, Spotlight, Border, or SamplesSpotlight.
    /// </summary>
    public void CleanLegacyFixtures()
    {
        if (!disableTemplateLights) return;

        if (originalLightingRoot != null)
        {
            originalLightingRoot.gameObject.SetActive(false);
        }

        // Disable all renderers across the scene matching legacy fixture patterns
        MeshRenderer[] allRenderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        foreach (MeshRenderer mr in allRenderers)
        {
            if (mr == null || mr.gameObject == null) continue;
            string objName = mr.gameObject.name.ToLower();
            string parentName = mr.transform.parent != null ? mr.transform.parent.name.ToLower() : "";

            if (objName.Contains("tripod") || objName.Contains("spotlight") ||
                objName.Contains("border") || objName.Contains("samplesspotlight") ||
                objName.Contains("backgroundmesh") ||
                parentName.Contains("tripod") || parentName.Contains("spotlight") ||
                parentName.Contains("border") || parentName.Contains("samplesspotlight"))
            {
                mr.enabled = false;
                mr.gameObject.SetActive(false);
            }
        }

        // Disable any legacy template lights
        Light[] allLights = Resources.FindObjectsOfTypeAll<Light>();
        foreach (Light l in allLights)
        {
            if (l == null || l.gameObject == null) continue;
            if (rigRoot != null && l.transform.IsChildOf(rigRoot)) continue;

            string n = l.gameObject.name.ToLower();
            if (n.Contains("spotlight") || n.Contains("tripod") || n.Contains("keylight") ||
                n.Contains("filllight") || n.Contains("backlight") || n.Contains("ceilinglight") ||
                n.Contains("samplesspotlight"))
            {
                if (!previousLightStates.ContainsKey(l))
                    previousLightStates[l] = l.enabled;
                l.enabled = false;
            }
        }
    }

    /// <summary>
    /// Builds a professional automotive photography lighting rig.
    /// Uses dual directional studio lights + overhead softbox + accent spots.
    /// </summary>
    private void BuildStudioLightingRig()
    {
        if (rigRoot != null)
        {
            Destroy(rigRoot.gameObject);
        }

        GameObject rig = new GameObject("AutoVerse_Studio_Lighting_Rig");
        rig.transform.SetParent(transform, false);
        rigRoot = rig.transform;

        // 1. Primary Directional Key Light (Angled front-left overhead)
        GameObject keyObj = new GameObject("Main_Studio_Key_Directional");
        keyObj.transform.SetParent(rigRoot, false);
        keyObj.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
        mainStudioKeyLight = keyObj.AddComponent<Light>();
        mainStudioKeyLight.type = LightType.Directional;
        mainStudioKeyLight.intensity = 2.0f;
        mainStudioKeyLight.shadows = LightShadows.Soft;

        // 2. Secondary Directional Fill Light (Angled rear-right overhead for 360 wrap)
        GameObject fillObj = new GameObject("Fill_Studio_Directional");
        fillObj.transform.SetParent(rigRoot, false);
        fillObj.transform.rotation = Quaternion.Euler(35f, 145f, 0f);
        fillStudioLight = fillObj.AddComponent<Light>();
        fillStudioLight.type = LightType.Directional;
        fillStudioLight.intensity = 1.2f;
        fillStudioLight.shadows = LightShadows.None;

        // 3. Overhead Studio Softbox Light (Directly above vehicle)
        GameObject softboxObj = new GameObject("Overhead_Studio_Softbox_Light");
        softboxObj.transform.SetParent(rigRoot, false);
        softboxObj.transform.position = new Vector3(0f, 6.5f, 0f);
        softboxObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        overheadSoftboxLight = softboxObj.AddComponent<Light>();
        overheadSoftboxLight.type = LightType.Spot;
        overheadSoftboxLight.spotAngle = 135f;
        overheadSoftboxLight.range = 20f;
        overheadSoftboxLight.intensity = 30f;
        overheadSoftboxLight.shadows = LightShadows.None;

        // 4. Front Accent Spot (Crisp front bumper / hood specular highlights)
        GameObject frontAccentObj = new GameObject("Front_Accent_Spot");
        frontAccentObj.transform.SetParent(rigRoot, false);
        frontAccentObj.transform.position = new Vector3(-3.5f, 3.8f, -4.5f);
        frontAccentLight = frontAccentObj.AddComponent<Light>();
        frontAccentLight.type = LightType.Spot;
        frontAccentLight.spotAngle = 75f;
        frontAccentLight.range = 15f;
        frontAccentLight.intensity = 22f;
        frontAccentLight.shadows = LightShadows.None;

        // 5. Rear Rim Accent Spot (Accentuates rear spoiler, shoulders, and silhouette)
        GameObject rimAccentObj = new GameObject("Rear_Rim_Accent_Spot");
        rimAccentObj.transform.SetParent(rigRoot, false);
        rimAccentObj.transform.position = new Vector3(4.0f, 4.2f, 4.5f);
        rimAccentLight = rimAccentObj.AddComponent<Light>();
        rimAccentLight.type = LightType.Spot;
        rimAccentLight.spotAngle = 80f;
        rimAccentLight.range = 16f;
        rimAccentLight.intensity = 25f;
        rimAccentLight.shadows = LightShadows.None;
    }

    private void Start()
    {
        CleanLegacyFixtures();
        SetLightingStyle((int)startingStyle);
    }

    private void Update()
    {
        UpdateAccentLightTargets();

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

        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            AdjustBrightness(0.1f);
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            AdjustBrightness(-0.1f);
    }

    private void UpdateAccentLightTargets()
    {
        Vector3 center = Vector3.zero;

        if (carSelector != null && carSelector.CurrentCar != null)
        {
            Renderer[] renderers = carSelector.CurrentCar.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                center = bounds.center;
            }
        }

        if (frontAccentLight != null)
        {
            frontAccentLight.transform.position = center + new Vector3(-3.5f, 3.8f, -4.5f);
            frontAccentLight.transform.LookAt(center + Vector3.up * 0.4f);
        }

        if (rimAccentLight != null)
        {
            rimAccentLight.transform.position = center + new Vector3(4.0f, 4.2f, 4.5f);
            rimAccentLight.transform.LookAt(center + Vector3.up * 0.5f);
        }

        if (overheadSoftboxLight != null)
        {
            overheadSoftboxLight.transform.position = center + new Vector3(0f, 6.5f, 0f);
            overheadSoftboxLight.transform.LookAt(center);
        }
    }

    public void SetLightingStyle(int index)
    {
        if (index < 0 || index > 3) return;

        currentStyle = (LightingStyle)index;
        ApplyCurrentStyle();

        // Notify environment manager to synchronize architectural wall illumination
        if (ShowroomEnvironmentManager.Instance != null)
        {
            ShowroomEnvironmentManager.Instance.UpdateDecorLighting(index);
        }
    }

    public void CycleLightingStyle()
    {
        int next = ((int)currentStyle + 1) % 4;
        SetLightingStyle(next);
    }

    private void ApplyCurrentStyle()
    {
        switch (currentStyle)
        {
            case LightingStyle.LuxuryStudio:
                // Warm, rich showroom photography with golden amber accents
                ApplyStyle(
                    new Color(1.0f, 0.96f, 0.90f),  // Key Light (Warm Champagne)
                    new Color(0.85f, 0.90f, 1.0f),   // Fill Light (Soft Cool Sky)
                    new Color(1.0f, 0.95f, 0.88f),   // Softbox Light (Overhead Studio)
                    new Color(1.0f, 0.82f, 0.50f),   // Front Accent
                    new Color(1.0f, 0.75f, 0.40f),   // Rear Rim Accent
                    2.1f, 1.25f, 32f, 24f, 26f,      // Intensities
                    new Color(0.46f, 0.48f, 0.54f),  // Ambient Light (No dark corners)
                    new Color(0.13f, 0.15f, 0.19f)); // Camera Background (Sleek Dark Slate)
                break;

            case LightingStyle.MidnightStudio:
                // High-tech, dramatic night showcase with ice-blue and cyan rim
                ApplyStyle(
                    new Color(0.90f, 0.94f, 1.0f),
                    new Color(0.25f, 0.55f, 0.95f),
                    new Color(0.70f, 0.85f, 1.0f),
                    new Color(0.20f, 0.80f, 1.0f),
                    new Color(0.15f, 0.70f, 1.0f),
                    1.9f, 1.15f, 28f, 22f, 30f,
                    new Color(0.36f, 0.42f, 0.52f),
                    new Color(0.08f, 0.10f, 0.14f));
                break;

            case LightingStyle.BrightGallery:
                // Modern art pavilion / high-key showroom: bright daylight white, maximum visibility
                ApplyStyle(
                    Color.white,
                    new Color(0.96f, 0.98f, 1.0f),
                    Color.white,
                    new Color(0.95f, 0.97f, 1.0f),
                    Color.white,
                    2.4f, 1.6f, 38f, 28f, 30f,
                    new Color(0.68f, 0.72f, 0.78f),  // Crisp bright ambient
                    new Color(0.72f, 0.75f, 0.80f)); // Clean modern gallery grey/white
                break;

            case LightingStyle.NeonGarage:
                // Cyberpunk / Underground tuner garage with electric magenta and neon cyan
                ApplyStyle(
                    new Color(1.0f, 0.92f, 0.98f),
                    new Color(0.15f, 0.45f, 1.0f),
                    new Color(0.90f, 0.35f, 0.90f),
                    new Color(0.0f, 0.90f, 1.0f),
                    new Color(1.0f, 0.10f, 0.70f),
                    2.0f, 1.3f, 30f, 25f, 32f,
                    new Color(0.38f, 0.32f, 0.48f),
                    new Color(0.10f, 0.08f, 0.15f));
                break;
        }
    }

    private void ApplyStyle(
        Color keyColor,
        Color fillColor,
        Color softboxColor,
        Color frontAccentColor,
        Color rimAccentColor,
        float keyIntensity,
        float fillIntensity,
        float softboxIntensity,
        float frontAccentIntensity,
        float rimAccentIntensity,
        Color ambientColor,
        Color backgroundColor)
    {
        if (mainStudioKeyLight != null)
        {
            mainStudioKeyLight.color = keyColor;
            mainStudioKeyLight.intensity = keyIntensity * masterBrightness;
        }

        if (fillStudioLight != null)
        {
            fillStudioLight.color = fillColor;
            fillStudioLight.intensity = fillIntensity * masterBrightness;
        }

        if (overheadSoftboxLight != null)
        {
            overheadSoftboxLight.color = softboxColor;
            overheadSoftboxLight.intensity = softboxIntensity * masterBrightness;
        }

        if (frontAccentLight != null)
        {
            frontAccentLight.color = frontAccentColor;
            frontAccentLight.intensity = frontAccentIntensity * masterBrightness;
        }

        if (rimAccentLight != null)
        {
            rimAccentLight.color = rimAccentColor;
            rimAccentLight.intensity = rimAccentIntensity * masterBrightness;
        }

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = 1.0f;

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = backgroundColor;
        }
    }

    public void SetBrightness(float value)
    {
        masterBrightness = Mathf.Clamp(value, 0.5f, 3.0f);
        ApplyCurrentStyle();
    }

    public void AdjustBrightness(float amount)
    {
        SetBrightness(masterBrightness + amount);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        foreach (var entry in previousLightStates)
        {
            if (entry.Key != null) entry.Key.enabled = entry.Value;
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
