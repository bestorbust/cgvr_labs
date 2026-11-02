using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AutoVerse - VehiclePainter
/// Manages body paint customization (18 colors, 4 finishes), wheel colors, caliper colors,
/// window tints, headlight styles, and accessory toggles across all showroom vehicles.
/// Uses Universal Render Pipeline Lit properties with standard fallback.
/// </summary>
public class VehiclePainter : MonoBehaviour
{
    public enum PaintFinish
    {
        Gloss,
        Metallic,
        Matte,
        Satin
    }

    [Header("References")]
    [SerializeField] private CarSelector carSelector;
    [SerializeField] private VehicleAccessoryManager accessoryManager;

    [Header("Paint Settings")]
    [SerializeField] private int defaultColorIndex = 0;
    [SerializeField] private PaintFinish defaultFinish = PaintFinish.Gloss;

    // 18 Selectable Body Colors
    private readonly Color[] bodyColors = new Color[]
    {
        new Color(0.96f, 0.96f, 0.98f), // 0: Pearl White
        new Color(0.08f, 0.08f, 0.09f), // 1: Midnight Black
        new Color(0.25f, 0.27f, 0.30f), // 2: Graphite
        new Color(0.82f, 0.84f, 0.86f), // 3: Arctic Silver
        new Color(0.85f, 0.12f, 0.12f), // 4: Racing Red
        new Color(0.55f, 0.00f, 0.09f), // 5: Crimson
        new Color(1.00f, 0.36f, 0.12f), // 6: Sunset Orange
        new Color(1.00f, 0.80f, 0.00f), // 7: Solar Yellow
        new Color(0.08f, 0.30f, 0.80f), // 8: Royal Blue
        new Color(0.22f, 0.72f, 1.00f), // 9: Sky Blue
        new Color(0.00f, 0.65f, 0.40f), // 10: Emerald Green
        new Color(0.12f, 0.32f, 0.18f), // 11: Forest Green
        new Color(0.45f, 0.08f, 0.72f), // 12: Violet
        new Color(0.75f, 0.45f, 0.50f), // 13: Rose Gold
        new Color(0.97f, 0.91f, 0.81f), // 14: Champagne
        new Color(0.58f, 0.38f, 0.20f), // 15: Bronze
        new Color(0.00f, 0.55f, 0.55f), // 16: Teal
        new Color(0.95f, 0.15f, 0.45f)  // 17: Candy Pink
    };

    private readonly string[] bodyColorNames = new string[]
    {
        "Pearl White",
        "Midnight Black",
        "Graphite",
        "Arctic Silver",
        "Racing Red",
        "Crimson",
        "Sunset Orange",
        "Solar Yellow",
        "Royal Blue",
        "Sky Blue",
        "Emerald Green",
        "Forest Green",
        "Violet",
        "Rose Gold",
        "Champagne",
        "Bronze",
        "Teal",
        "Candy Pink"
    };

    private readonly string[] finishNames = new string[]
    {
        "Gloss",
        "Metallic",
        "Matte",
        "Satin"
    };

    // Wheel Colors: Black, Silver, Gunmetal, Gold, Bronze
    private readonly Color[] wheelColors = new Color[]
    {
        new Color(0.08f, 0.08f, 0.08f), // Black
        new Color(0.88f, 0.88f, 0.90f), // Silver
        new Color(0.30f, 0.32f, 0.35f), // Gunmetal
        new Color(0.88f, 0.70f, 0.22f), // Gold
        new Color(0.60f, 0.42f, 0.24f)  // Bronze
    };

    private readonly string[] wheelColorNames = new string[]
    {
        "Black",
        "Silver",
        "Gunmetal",
        "Gold",
        "Bronze"
    };

    // Brake Caliper Colors: Racing Red, Acid Yellow, Electric Blue, Acid Green, Stealth Black
    private readonly Color[] caliperColors = new Color[]
    {
        new Color(0.90f, 0.08f, 0.08f), // Racing Red
        new Color(1.00f, 0.82f, 0.00f), // Acid Yellow
        new Color(0.00f, 0.65f, 1.00f), // Electric Blue
        new Color(0.15f, 0.95f, 0.25f), // Acid Green
        new Color(0.10f, 0.10f, 0.12f)  // Stealth Black
    };

    private readonly string[] caliperColorNames = new string[]
    {
        "Brembo Red",
        "Acid Yellow",
        "Electric Blue",
        "Acid Green",
        "Stealth Black"
    };

    // Window Tints: Clear, Light Smoke, Dark Charcoal, Limo, Sunset Gold Chameleon, Cyber Blue Mirror
    private readonly Color[] windowTints = new Color[]
    {
        new Color(0.85f, 0.92f, 0.98f, 0.35f), // Clear
        new Color(0.38f, 0.42f, 0.48f, 0.55f), // Light Smoke
        new Color(0.12f, 0.14f, 0.18f, 0.82f), // Dark Charcoal
        new Color(0.04f, 0.04f, 0.05f, 0.96f), // Limo Blackout
        new Color(0.85f, 0.65f, 0.25f, 0.70f), // Sunset Gold
        new Color(0.15f, 0.55f, 0.95f, 0.75f)  // Cyber Blue
    };

    private readonly string[] windowTintNames = new string[]
    {
        "Clear",
        "Light Smoke",
        "Dark Charcoal",
        "Limo Blackout",
        "Sunset Gold",
        "Cyber Blue"
    };

    // Headlight Styles: Xenon White, Warm Halogen, Ice Blue LED, Cyber Violet, Amber Gold DRL
    private readonly Color[] headlightColors = new Color[]
    {
        new Color(1.0f, 1.0f, 1.0f),       // Xenon White
        new Color(1.0f, 0.92f, 0.75f),     // Warm Halogen
        new Color(0.65f, 0.88f, 1.0f),     // Ice Blue LED
        new Color(0.85f, 0.45f, 1.0f),     // Cyber Violet
        new Color(1.0f, 0.75f, 0.15f)      // Amber Gold
    };

    private readonly Color[] headlightEmissions = new Color[]
    {
        new Color(1.8f, 1.8f, 1.8f),
        new Color(1.8f, 1.6f, 1.2f),
        new Color(1.0f, 1.6f, 2.2f),
        new Color(1.8f, 0.8f, 2.2f),
        new Color(2.0f, 1.4f, 0.3f)
    };

    private readonly string[] headlightStyleNames = new string[]
    {
        "Xenon White",
        "Warm Halogen",
        "Ice Blue LED",
        "Cyber Violet",
        "Amber Gold"
    };

    // Underglow Colors
    private readonly Color[] underglowColors = new Color[]
    {
        new Color(0.00f, 0.85f, 1.00f), // Electric Cyan
        new Color(1.00f, 0.08f, 0.65f), // Neon Magenta
        new Color(0.20f, 1.00f, 0.20f), // Acid Green
        new Color(1.00f, 0.65f, 0.00f), // Amber Gold
        new Color(0.80f, 0.92f, 1.00f), // Ice White
        new Color(0.60f, 0.05f, 1.00f)  // Royal Violet
    };

    private readonly string[] underglowColorNames = new string[]
    {
        "Electric Cyan",
        "Neon Magenta",
        "Acid Green",
        "Amber Gold",
        "Ice White",
        "Royal Violet"
    };

    // Original vehicle shared materials cache (for reset)
    private readonly Dictionary<GameObject, Material[]> originalMaterials =
        new Dictionary<GameObject, Material[]>();

    // Current working cloned materials per car
    private readonly Dictionary<GameObject, Material[]> paintMaterials =
        new Dictionary<GameObject, Material[]>();

    private class MaterialSnapshot
    {
        public Color baseColor;
        public float metallic;
        public float smoothness;
        public Color emissionColor;
        public bool hasEmission;
    }

    private readonly Dictionary<GameObject, MaterialSnapshot[]> originalSnapshots =
        new Dictionary<GameObject, MaterialSnapshot[]>();

    // Selected state trackers per car
    private readonly Dictionary<GameObject, int> selectedBodyColors = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, PaintFinish> selectedFinishes = new Dictionary<GameObject, PaintFinish>();
    private readonly Dictionary<GameObject, int> selectedWheelColors = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> selectedCaliperColors = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> selectedWindowTints = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> selectedHeadlightStyles = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> selectedUnderglowColors = new Dictionary<GameObject, int>();

    private GameObject lastTrackedCar;

    #region Public Properties

    public int BodyColorCount => bodyColors.Length;
    public int WheelColorCount => wheelColors.Length;
    public int CaliperColorCount => caliperColors.Length;
    public int WindowTintCount => windowTints.Length;
    public int HeadlightStyleCount => headlightColors.Length;
    public int UnderglowColorCount => underglowColors.Length;

    public string[] BodyColorNames => (string[])bodyColorNames.Clone();
    public string[] FinishNames => (string[])finishNames.Clone();
    public string[] WheelColorNames => (string[])wheelColorNames.Clone();
    public string[] CaliperColorNames => (string[])caliperColorNames.Clone();
    public string[] WindowTintNames => (string[])windowTintNames.Clone();
    public string[] HeadlightStyleNames => (string[])headlightStyleNames.Clone();
    public string[] UnderglowColorNames => (string[])underglowColorNames.Clone();

    public Color[] BodyColors => (Color[])bodyColors.Clone();
    public Color[] WheelColors => (Color[])wheelColors.Clone();
    public Color[] CaliperColors => (Color[])caliperColors.Clone();
    public Color[] WindowTints => (Color[])windowTints.Clone();
    public Color[] HeadlightColors => (Color[])headlightColors.Clone();
    public Color[] UnderglowColors => (Color[])underglowColors.Clone();

    public string[] GetFinishNames() => (string[])finishNames.Clone();
    public string[] GetBodyColorNames() => (string[])bodyColorNames.Clone();
    public Color[] GetBodyColors() => (Color[])bodyColors.Clone();
    public string[] GetWheelColorNames() => (string[])wheelColorNames.Clone();
    public Color[] GetWheelColors() => (Color[])wheelColors.Clone();
    public string[] GetCaliperColorNames() => (string[])caliperColorNames.Clone();
    public Color[] GetCaliperColors() => (Color[])caliperColors.Clone();
    public string[] GetWindowTintNames() => (string[])windowTintNames.Clone();
    public Color[] GetWindowTints() => (Color[])windowTints.Clone();
    public string[] GetHeadlightStyleNames() => (string[])headlightStyleNames.Clone();
    public Color[] GetHeadlightColors() => (Color[])headlightColors.Clone();
    public string[] GetUnderglowColorNames() => (string[])underglowColorNames.Clone();
    public Color[] GetUnderglowColors() => (Color[])underglowColors.Clone();

    public int CurrentBodyColorIndex => GetSelectedBodyColorIndex(carSelector != null ? carSelector.CurrentCar : null);
    public PaintFinish CurrentFinish => GetSelectedFinish(carSelector != null ? carSelector.CurrentCar : null);
    public int CurrentWheelColorIndex => GetSelectedWheelColorIndex(carSelector != null ? carSelector.CurrentCar : null);
    public int CurrentCaliperColorIndex => GetSelectedCaliperColorIndex(carSelector != null ? carSelector.CurrentCar : null);
    public int CurrentWindowTintIndex => GetSelectedWindowTintIndex(carSelector != null ? carSelector.CurrentCar : null);
    public int CurrentHeadlightIndex => GetSelectedHeadlightStyleIndex(carSelector != null ? carSelector.CurrentCar : null);
    public int CurrentUnderglowColorIndex => GetSelectedUnderglowColorIndex(carSelector != null ? carSelector.CurrentCar : null);

    public bool SupportsGlass() => SupportsGlass(carSelector != null ? carSelector.CurrentCar : null);
    public bool SupportsHeadlights() => SupportsHeadlights(carSelector != null ? carSelector.CurrentCar : null);

    public int GetSelectedBodyColorIndex(GameObject car) =>
        car != null && selectedBodyColors.ContainsKey(car) ? selectedBodyColors[car] : defaultColorIndex;

    public PaintFinish GetSelectedFinish(GameObject car) =>
        car != null && selectedFinishes.ContainsKey(car) ? selectedFinishes[car] : defaultFinish;

    public int GetSelectedWheelColorIndex(GameObject car) =>
        car != null && selectedWheelColors.ContainsKey(car) ? selectedWheelColors[car] : 1;

    public int GetSelectedCaliperColorIndex(GameObject car) =>
        car != null && selectedCaliperColors.ContainsKey(car) ? selectedCaliperColors[car] : 0;

    public int GetSelectedWindowTintIndex(GameObject car) =>
        car != null && selectedWindowTints.ContainsKey(car) ? selectedWindowTints[car] : 0;

    public int GetSelectedHeadlightStyleIndex(GameObject car) =>
        car != null && selectedHeadlightStyles.ContainsKey(car) ? selectedHeadlightStyles[car] : 0;

    public int GetSelectedUnderglowColorIndex(GameObject car) =>
        car != null && selectedUnderglowColors.ContainsKey(car) ? selectedUnderglowColors[car] : 0;

    public bool SupportsGlass(GameObject car)
    {
        if (car == null) return false;
        if (car.name.ToLower().Contains("race")) return false;
        return true;
    }

    public bool SupportsHeadlights(GameObject car)
    {
        if (car == null) return false;
        if (car.name.ToLower().Contains("race")) return false;
        return true;
    }

    #endregion

    private void Start()
    {
        if (carSelector == null)
            carSelector = FindAnyObjectByType<CarSelector>();

        if (carSelector == null)
        {
            Debug.LogError("VehiclePainter: CarSelector not found.");
            enabled = false;
            return;
        }

        if (accessoryManager == null)
        {
            accessoryManager = GetComponent<VehicleAccessoryManager>();
            if (accessoryManager == null)
                accessoryManager = FindAnyObjectByType<VehicleAccessoryManager>();
            if (accessoryManager == null)
                accessoryManager = gameObject.AddComponent<VehicleAccessoryManager>();
        }

        // Auto-ensure ShowroomLightingManager is active
        if (GetComponent<ShowroomLightingManager>() == null && FindAnyObjectByType<ShowroomLightingManager>() == null)
        {
            gameObject.AddComponent<ShowroomLightingManager>();
        }

        // Auto-ensure showroom environment decor and UI manager are active
        if (GetComponent<ShowroomEnvironmentManager>() == null && FindAnyObjectByType<ShowroomEnvironmentManager>() == null)
        {
            gameObject.AddComponent<ShowroomEnvironmentManager>();
        }

        if (GetComponent<AutoVerseShowroomUI>() == null && FindAnyObjectByType<AutoVerseShowroomUI>() == null)
        {
            gameObject.AddComponent<AutoVerseShowroomUI>();
        }

        SetupCurrentCar();
        if (carSelector != null)
        {
            carSelector.OnCarChanged += HandleCarChanged;
        }

        if (carSelector.CurrentCar != null && accessoryManager != null)
        {
            accessoryManager.OnVehicleChanged(carSelector.CurrentCar);
        }
    }

    private void HandleCarChanged(GameObject newCar)
    {
        if (newCar == null) return;
        lastTrackedCar = newCar;
        SetupCurrentCar();
        if (accessoryManager != null)
            accessoryManager.OnVehicleChanged(newCar);
    }

    private void Update()
    {
        if (carSelector == null || carSelector.CurrentCar == null)
            return;

        GameObject activeCar = carSelector.CurrentCar;
        if (activeCar != lastTrackedCar)
        {
            lastTrackedCar = activeCar;
            SetupCurrentCar();
            if (accessoryManager != null)
                accessoryManager.OnVehicleChanged(activeCar);
        }

        HandleKeyboardInputs();
    }

    private void HandleKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.C))
            NextColor();

        if (Input.GetKeyDown(KeyCode.G))
            SetFinish(PaintFinish.Gloss);
        if (Input.GetKeyDown(KeyCode.M))
            SetFinish(PaintFinish.Metallic);
        if (Input.GetKeyDown(KeyCode.N))
            SetFinish(PaintFinish.Matte);
        if (Input.GetKeyDown(KeyCode.S))
            SetFinish(PaintFinish.Satin);

        if (Input.GetKeyDown(KeyCode.W))
            NextWheelColor();
        if (Input.GetKeyDown(KeyCode.J))
            NextWindowTint();
        if (Input.GetKeyDown(KeyCode.H))
            NextHeadlightStyle();

        if (Input.GetKeyDown(KeyCode.K))
            ToggleAccessory(VehicleAccessoryManager.ACC_ROOF_LIGHT_BAR);
        if (Input.GetKeyDown(KeyCode.P))
            ToggleAccessory(VehicleAccessoryManager.ACC_REAR_SPOILER);
        if (Input.GetKeyDown(KeyCode.I))
            ToggleAccessory(VehicleAccessoryManager.ACC_RACING_STRIPES);
        if (Input.GetKeyDown(KeyCode.U))
            ToggleAccessory(VehicleAccessoryManager.ACC_UNDERGLOW);

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Backspace))
            ResetCustomization();
    }

    private void SetupCurrentCar()
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null || originalMaterials.ContainsKey(car))
            return;

        Renderer renderer = GetVehicleRenderer(car);
        if (renderer == null)
            return;

        Material[] originals = renderer.sharedMaterials;
        Material[] copies = new Material[originals.Length];
        MaterialSnapshot[] snapshots = new MaterialSnapshot[originals.Length];

        for (int i = 0; i < originals.Length; i++)
        {
            if (originals[i] != null)
            {
                copies[i] = new Material(originals[i]);

                MaterialSnapshot snap = new MaterialSnapshot();
                if (originals[i].HasProperty("_BaseColor")) snap.baseColor = originals[i].GetColor("_BaseColor");
                else if (originals[i].HasProperty("_Color")) snap.baseColor = originals[i].color;

                if (originals[i].HasProperty("_Metallic")) snap.metallic = originals[i].GetFloat("_Metallic");
                if (originals[i].HasProperty("_Smoothness")) snap.smoothness = originals[i].GetFloat("_Smoothness");

                if (originals[i].HasProperty("_EmissionColor")) snap.emissionColor = originals[i].GetColor("_EmissionColor");
                snap.hasEmission = originals[i].IsKeywordEnabled("_EMISSION");

                snapshots[i] = snap;
            }
        }

        originalMaterials[car] = originals;
        paintMaterials[car] = copies;
        originalSnapshots[car] = snapshots;

        selectedBodyColors[car] = defaultColorIndex;
        selectedFinishes[car] = defaultFinish;
        selectedWheelColors[car] = 1; // Default Silver
        selectedCaliperColors[car] = 0; // Default Brembo Red
        selectedWindowTints[car] = 0; // Default Clear
        selectedHeadlightStyles[car] = 0; // Default White
        selectedUnderglowColors[car] = 0; // Default Electric Cyan

        renderer.materials = copies;
    }

    private Renderer GetVehicleRenderer(GameObject car)
    {
        if (car == null) return null;

        Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.gameObject.name.StartsWith("Housing") ||
                r.gameObject.name.StartsWith("Wing") ||
                r.gameObject.name.StartsWith("Stripe") ||
                r.gameObject.name.StartsWith("Neon") ||
                (accessoryManager != null && r.transform.IsChildOf(accessoryManager.transform)))
            {
                continue;
            }
            return r;
        }
        return car.GetComponentInChildren<Renderer>();
    }

    #region Public Customization APIs

    public void SetColor(int colorIndex) => SetBodyColor(colorIndex);

    public void SetBodyColor(int index)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedBodyColors[car] = Mathf.Clamp(index, 0, bodyColors.Length - 1);
        ApplyBodyPaint(car);
    }

    public void NextColor()
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedBodyColors[car] = (selectedBodyColors[car] + 1) % bodyColors.Length;
        ApplyBodyPaint(car);
    }

    public void SetFinish(PaintFinish finish)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedFinishes[car] = finish;
        ApplyBodyPaint(car);
    }

    public void SetPaintFinish(int index)
    {
        if (index < 0 || index > 3) return;
        SetFinish((PaintFinish)index);
    }

    public void SetWheelColor(int index)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedWheelColors[car] = Mathf.Clamp(index, 0, wheelColors.Length - 1);
        ApplyWheelColor(car);
    }

    public void NextWheelColor()
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedWheelColors[car] = (selectedWheelColors[car] + 1) % wheelColors.Length;
        ApplyWheelColor(car);
    }

    public void SetCaliperColor(int index)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedCaliperColors[car] = Mathf.Clamp(index, 0, caliperColors.Length - 1);
        ApplyWheelColor(car);
    }

    public void SetWindowTint(int index)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        if (!SupportsGlass(car)) return;

        SetupCurrentCar();
        selectedWindowTints[car] = Mathf.Clamp(index, 0, windowTints.Length - 1);
        ApplyWindowTint(car);
    }

    public void NextWindowTint()
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null || !SupportsGlass(car)) return;

        SetupCurrentCar();
        selectedWindowTints[car] = (selectedWindowTints[car] + 1) % windowTints.Length;
        ApplyWindowTint(car);
    }

    public void SetHeadlightStyle(int index)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null || !SupportsHeadlights(car)) return;

        SetupCurrentCar();
        selectedHeadlightStyles[car] = Mathf.Clamp(index, 0, headlightColors.Length - 1);
        ApplyHeadlights(car);
    }

    public void NextHeadlightStyle()
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null || !SupportsHeadlights(car)) return;

        SetupCurrentCar();
        selectedHeadlightStyles[car] = (selectedHeadlightStyles[car] + 1) % headlightColors.Length;
        ApplyHeadlights(car);
    }

    public void SetUnderglowColor(int index)
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null) return;

        SetupCurrentCar();
        selectedUnderglowColors[car] = Mathf.Clamp(index, 0, underglowColors.Length - 1);
        Color c = underglowColors[selectedUnderglowColors[car]];

        // If Underglow accessory is active or toggled, update color in accessory manager
        if (accessoryManager != null)
        {
            if (!accessoryManager.IsAccessoryActive(VehicleAccessoryManager.ACC_UNDERGLOW))
            {
                accessoryManager.ToggleAccessory(VehicleAccessoryManager.ACC_UNDERGLOW);
            }
            accessoryManager.SetUnderglowColor(c);
        }
    }

    public void ToggleAccessory(int index)
    {
        if (accessoryManager != null)
        {
            accessoryManager.ToggleAccessory(index);
        }
    }

    public bool IsAccessoryActive(int index)
    {
        return accessoryManager != null && accessoryManager.IsAccessoryActive(index);
    }

    public void ResetCustomization()
    {
        GameObject car = carSelector.CurrentCar;
        if (car == null || !paintMaterials.ContainsKey(car) || !originalSnapshots.ContainsKey(car))
            return;

        Renderer renderer = GetVehicleRenderer(car);
        if (renderer == null) return;

        Material[] materials = paintMaterials[car];
        MaterialSnapshot[] snapshots = originalSnapshots[car];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null || i >= snapshots.Length) continue;

            MaterialSnapshot snap = snapshots[i];
            SetColorProperty(mat, snap.baseColor);
            SetSurface(mat, snap.metallic, snap.smoothness);

            if (snap.hasEmission)
            {
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", snap.emissionColor);
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }
        }

        renderer.materials = materials;

        selectedBodyColors[car] = defaultColorIndex;
        selectedFinishes[car] = defaultFinish;
        selectedWheelColors[car] = 1;
        selectedCaliperColors[car] = 0;
        selectedWindowTints[car] = 0;
        selectedHeadlightStyles[car] = 0;
        selectedUnderglowColors[car] = 0;

        if (accessoryManager != null)
        {
            accessoryManager.ResetAllAccessories();
        }
    }

    #endregion

    #region Material Application Logic

    private void ApplyBodyPaint(GameObject car)
    {
        Renderer renderer = GetVehicleRenderer(car);
        if (renderer == null || !paintMaterials.ContainsKey(car)) return;

        Material[] materials = paintMaterials[car];
        Color color = bodyColors[selectedBodyColors[car]];
        PaintFinish finish = selectedFinishes[car];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null) continue;

            if (IsBodySlot(car.name, mat.name, i))
            {
                SetColorProperty(mat, color);

                switch (finish)
                {
                    case PaintFinish.Gloss:
                        SetSurface(mat, 0.08f, 0.90f);
                        break;
                    case PaintFinish.Metallic:
                        SetSurface(mat, 0.90f, 0.85f);
                        break;
                    case PaintFinish.Matte:
                        SetSurface(mat, 0.0f, 0.15f);
                        break;
                    case PaintFinish.Satin:
                        SetSurface(mat, 0.45f, 0.50f);
                        break;
                }
            }
        }

        renderer.materials = materials;
    }

    private void ApplyWheelColor(GameObject car)
    {
        Renderer renderer = GetVehicleRenderer(car);
        if (renderer == null || !paintMaterials.ContainsKey(car)) return;

        Material[] materials = paintMaterials[car];
        Color color = wheelColors[selectedWheelColors[car]];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null) continue;

            if (IsWheelSlot(car.name, mat.name, i))
            {
                SetColorProperty(mat, color);
                SetSurface(mat, 0.88f, 0.75f);
            }
        }

        renderer.materials = materials;
    }

    private void ApplyWindowTint(GameObject car)
    {
        if (!SupportsGlass(car)) return;

        Renderer renderer = GetVehicleRenderer(car);
        if (renderer == null || !paintMaterials.ContainsKey(car)) return;

        Material[] materials = paintMaterials[car];
        Color tint = windowTints[selectedWindowTints[car]];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null) continue;

            if (IsGlassSlot(car.name, mat.name, i))
            {
                SetColorProperty(mat, tint);
                SetSurface(mat, 0.1f, 0.95f);
            }
        }

        renderer.materials = materials;
    }

    private void ApplyHeadlights(GameObject car)
    {
        if (!SupportsHeadlights(car)) return;

        Renderer renderer = GetVehicleRenderer(car);
        if (renderer == null || !paintMaterials.ContainsKey(car)) return;

        Material[] materials = paintMaterials[car];
        int styleIndex = selectedHeadlightStyles[car];
        Color color = headlightColors[styleIndex];
        Color emission = headlightEmissions[styleIndex];

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null) continue;

            if (IsHeadlightSlot(car.name, mat.name, i))
            {
                SetColorProperty(mat, color);
                mat.EnableKeyword("_EMISSION");
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", emission);
                SetSurface(mat, 0.1f, 0.90f);
            }
        }

        renderer.materials = materials;
    }

    #endregion

    #region Slot Identification Helpers

    private bool IsBodySlot(string carName, string matName, int slotIndex)
    {
        carName = carName.ToLower();
        matName = matName.ToLower();

        if (carName.Contains("golf"))
        {
            return slotIndex == 3 || matName.Contains("lime");
        }

        if (slotIndex == 0)
            return true;

        return matName.Contains("body") || matName.Contains("paint") || matName.Contains("car");
    }

    private bool IsGlassSlot(string carName, string matName, int slotIndex)
    {
        carName = carName.ToLower();
        if (carName.Contains("race")) return false;

        matName = matName.ToLower();
        return matName.Contains("glass") || matName.Contains("window");
    }

    private bool IsWheelSlot(string carName, string matName, int slotIndex)
    {
        matName = matName.ToLower();
        return matName.Contains("hub") || matName.Contains("wheel") || matName.Contains("rim");
    }

    private bool IsHeadlightSlot(string carName, string matName, int slotIndex)
    {
        carName = carName.ToLower();
        if (carName.Contains("race")) return false;

        matName = matName.ToLower();
        return matName.Contains("headlight");
    }

    private void SetColorProperty(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color"))
            mat.color = color;
    }

    private void SetSurface(Material material, float metallic, float smoothness)
    {
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);

        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);
    }

    #endregion

    private void OnDestroy()
    {
        if (carSelector != null)
        {
            carSelector.OnCarChanged -= HandleCarChanged;
        }

        foreach (Material[] materials in paintMaterials.Values)
        {
            if (materials == null) continue;
            foreach (Material m in materials)
            {
                if (m != null) Destroy(m);
            }
        }
        paintMaterials.Clear();
    }
}
