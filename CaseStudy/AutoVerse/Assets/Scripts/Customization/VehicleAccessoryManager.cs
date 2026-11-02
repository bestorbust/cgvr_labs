using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages procedural showroom accessories (Roof Light Bar, Rear Spoiler, Racing Stripes, Underglow)
/// dynamically fitted to the active vehicle's bounding box.
/// </summary>
public class VehicleAccessoryManager : MonoBehaviour
{
    public const int ACCESSORY_COUNT = 4;
    public const int ACC_ROOF_LIGHT_BAR = 0;
    public const int ACC_REAR_SPOILER = 1;
    public const int ACC_RACING_STRIPES = 2;
    public const int ACC_UNDERGLOW = 3;

    private readonly string[] accessoryNames = new string[]
    {
        "Roof Light Bar",
        "Rear Spoiler",
        "Racing Stripes",
        "Underglow Lighting"
    };

    [Header("Settings")]
    [SerializeField] private float platformTopY = 0f;

    private readonly bool[] accessoryStates = new bool[ACCESSORY_COUNT];
    private GameObject accessoryRoot;
    private GameObject[] accessoryObjects = new GameObject[ACCESSORY_COUNT];
    private readonly List<Material> createdMaterials = new List<Material>();

    private GameObject currentCar;
    private Bounds currentBounds;

    public string[] GetAccessoryNames() => (string[])accessoryNames.Clone();

    public bool IsAccessoryActive(int index)
    {
        if (index < 0 || index >= ACCESSORY_COUNT) return false;
        return accessoryStates[index];
    }

    private void Awake()
    {
        EnsureRootExists();
    }

    private void EnsureRootExists()
    {
        if (accessoryRoot == null)
        {
            accessoryRoot = new GameObject("[AutoVerse_Accessories]");
            accessoryRoot.transform.SetParent(transform, false);
        }
    }

    public void OnVehicleChanged(GameObject car)
    {
        currentCar = car;
        if (currentCar == null)
        {
            SetAllObjectsActive(false);
            return;
        }

        CalculateCarBounds();
        RebuildAccessories();
        UpdateVisibility();
    }

    private void CalculateCarBounds()
    {
        if (currentCar == null) return;

        Renderer[] renderers = currentCar.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            currentBounds = new Bounds(currentCar.transform.position, Vector3.one);
            return;
        }

        // Only include vehicle renderers, ignore our accessories
        Bounds b = new Bounds(renderers[0].bounds.center, Vector3.zero);
        bool found = false;

        foreach (Renderer r in renderers)
        {
            if (r.transform.IsChildOf(accessoryRoot.transform)) continue;
            if (!found)
            {
                b = r.bounds;
                found = true;
            }
            else
            {
                b.Encapsulate(r.bounds);
            }
        }

        currentBounds = b;
    }

    public void ToggleAccessory(int index)
    {
        if (index < 0 || index >= ACCESSORY_COUNT) return;

        accessoryStates[index] = !accessoryStates[index];
        UpdateVisibility();

        Debug.Log($"Accessory [{accessoryNames[index]}]: {(accessoryStates[index] ? "ON" : "OFF")}");
    }

    public void SetAccessory(int index, bool active)
    {
        if (index < 0 || index >= ACCESSORY_COUNT) return;

        accessoryStates[index] = active;
        UpdateVisibility();
    }

    public void ResetAccessories()
    {
        for (int i = 0; i < ACCESSORY_COUNT; i++)
        {
            accessoryStates[i] = false;
        }
        UpdateVisibility();
        Debug.Log("AutoVerse: Accessories reset.");
    }

    public void ResetAllAccessories() => ResetAccessories();

    public void SetUnderglowColor(Color c)
    {
        if (accessoryObjects[ACC_UNDERGLOW] == null) return;

        Renderer[] renderers = accessoryObjects[ACC_UNDERGLOW].GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.material != null)
            {
                if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", c);
                else if (r.material.HasProperty("_Color")) r.material.color = c;
                if (r.material.HasProperty("_EmissionColor")) r.material.SetColor("_EmissionColor", c * 2.5f);
            }
        }

        Light[] lights = accessoryObjects[ACC_UNDERGLOW].GetComponentsInChildren<Light>();
        foreach (Light l in lights)
        {
            l.color = c;
        }
    }

    private void UpdateVisibility()
    {
        for (int i = 0; i < ACCESSORY_COUNT; i++)
        {
            if (accessoryObjects[i] != null)
            {
                accessoryObjects[i].SetActive(accessoryStates[i] && currentCar != null);
            }
        }
    }

    private void SetAllObjectsActive(bool active)
    {
        for (int i = 0; i < ACCESSORY_COUNT; i++)
        {
            if (accessoryObjects[i] != null)
                accessoryObjects[i].SetActive(active);
        }
    }

    private void RebuildAccessories()
    {
        EnsureRootExists();

        // Destroy previous accessory meshes
        for (int i = 0; i < ACCESSORY_COUNT; i++)
        {
            if (accessoryObjects[i] != null)
            {
                Destroy(accessoryObjects[i]);
                accessoryObjects[i] = null;
            }
        }

        if (currentCar == null) return;

        // Parent accessory root under current car so they move and rotate together
        accessoryRoot.transform.SetParent(currentCar.transform, true);

        // Build 0: Roof Light Bar
        accessoryObjects[ACC_ROOF_LIGHT_BAR] = BuildRoofLightBar();

        // Build 1: Rear Spoiler
        accessoryObjects[ACC_REAR_SPOILER] = BuildRearSpoiler();

        // Build 2: Racing Stripes
        accessoryObjects[ACC_RACING_STRIPES] = BuildRacingStripes();

        // Build 3: Underglow Lighting
        accessoryObjects[ACC_UNDERGLOW] = BuildUnderglow();
    }

    private Material CreateLitMaterial(string name, Color color, float metallic, float smoothness, Color? emission = null)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material mat = new Material(shader)
        {
            name = name
        };

        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        else if (mat.HasProperty("_Color")) mat.color = color;

        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emission.Value);
        }

        createdMaterials.Add(mat);
        return mat;
    }

    private GameObject BuildRoofLightBar()
    {
        GameObject barRoot = new GameObject("Roof_Light_Bar");
        barRoot.transform.SetParent(accessoryRoot.transform, false);

        float carWidth = currentBounds.size.x;
        float barWidth = Mathf.Clamp(carWidth * 0.65f, 0.6f, 1.4f);
        float roofY = currentBounds.max.y;
        float centerZ = currentBounds.center.z;
        float barZ = centerZ + currentBounds.extents.z * 0.05f;

        // For golf cart or specific tall models, roof is top
        Vector3 barPos = new Vector3(currentBounds.center.x, roofY + 0.04f, barZ);

        Material housingMat = CreateLitMaterial("LightBar_Housing", new Color(0.12f, 0.12f, 0.14f), 0.8f, 0.5f);
        Material ledMat = CreateLitMaterial("LightBar_LED", new Color(1f, 0.75f, 0.2f), 0.2f, 0.9f, new Color(1.8f, 1.2f, 0.2f));

        // Housing
        GameObject housing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        housing.name = "Housing";
        housing.transform.SetParent(barRoot.transform, false);
        housing.transform.position = barPos;
        housing.transform.localScale = new Vector3(barWidth, 0.06f, 0.10f);
        housing.GetComponent<Renderer>().sharedMaterial = housingMat;
        RemoveCollider(housing);

        // Mount legs
        float legOffset = barWidth * 0.42f;
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.name = $"MountLeg_{(side < 0 ? "L" : "R")}";
            leg.transform.SetParent(barRoot.transform, false);
            leg.transform.position = barPos + new Vector3(side * legOffset, -0.04f, 0f);
            leg.transform.localScale = new Vector3(0.04f, 0.08f, 0.06f);
            leg.GetComponent<Renderer>().sharedMaterial = housingMat;
            RemoveCollider(leg);
        }

        // LED Lenses
        int ledCount = 5;
        float spacing = (barWidth * 0.8f) / (ledCount - 1);
        float startX = -((barWidth * 0.8f) * 0.5f);

        for (int i = 0; i < ledCount; i++)
        {
            GameObject led = GameObject.CreatePrimitive(PrimitiveType.Cube);
            led.name = $"LED_{i}";
            led.transform.SetParent(barRoot.transform, false);
            led.transform.position = barPos + new Vector3(startX + i * spacing, 0f, 0.045f);
            led.transform.localScale = new Vector3(spacing * 0.6f, 0.035f, 0.02f);
            led.GetComponent<Renderer>().sharedMaterial = ledMat;
            RemoveCollider(led);
        }

        return barRoot;
    }

    private GameObject BuildRearSpoiler()
    {
        // Formula race car already has its own prominent aerodynamic rear wing; avoid awkward clipping
        if (currentCar != null && currentCar.name.ToLower().Contains("race"))
        {
            GameObject dummy = new GameObject("Rear_Spoiler_Disabled");
            dummy.transform.SetParent(accessoryRoot.transform, false);
            return dummy;
        }

        GameObject spoilerRoot = new GameObject("Rear_Spoiler");
        spoilerRoot.transform.SetParent(accessoryRoot.transform, false);

        float carWidth = currentBounds.size.x;
        float wingWidth = Mathf.Clamp(carWidth * 0.82f, 0.8f, 1.8f);
        float rearZ = currentBounds.min.z + 0.12f;
        float deckY = currentBounds.min.y + currentBounds.size.y * 0.62f;

        Vector3 wingPos = new Vector3(currentBounds.center.x, deckY + 0.18f, rearZ);

        Material wingMat = CreateLitMaterial("Spoiler_Wing", new Color(0.10f, 0.10f, 0.12f), 0.7f, 0.75f);

        // Wing Blade
        GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blade.name = "Wing_Blade";
        blade.transform.SetParent(spoilerRoot.transform, false);
        blade.transform.position = wingPos;
        blade.transform.rotation = Quaternion.Euler(-8f, 0f, 0f);
        blade.transform.localScale = new Vector3(wingWidth, 0.035f, 0.22f);
        blade.GetComponent<Renderer>().sharedMaterial = wingMat;
        RemoveCollider(blade);

        // End Plates
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = $"EndPlate_{(side < 0 ? "L" : "R")}";
            plate.transform.SetParent(spoilerRoot.transform, false);
            plate.transform.position = wingPos + new Vector3(side * (wingWidth * 0.5f), 0.02f, 0f);
            plate.transform.localScale = new Vector3(0.02f, 0.12f, 0.24f);
            plate.GetComponent<Renderer>().sharedMaterial = wingMat;
            RemoveCollider(plate);
        }

        // Uprights / Struts
        float strutOffset = wingWidth * 0.28f;
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject strut = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strut.name = $"Strut_{(side < 0 ? "L" : "R")}";
            strut.transform.SetParent(spoilerRoot.transform, false);
            strut.transform.position = wingPos + new Vector3(side * strutOffset, -0.09f, -0.02f);
            strut.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
            strut.transform.localScale = new Vector3(0.03f, 0.20f, 0.06f);
            strut.GetComponent<Renderer>().sharedMaterial = wingMat;
            RemoveCollider(strut);
        }

        return spoilerRoot;
    }

    private GameObject BuildRacingStripes()
    {
        GameObject stripesRoot = new GameObject("Racing_Stripes");
        stripesRoot.transform.SetParent(accessoryRoot.transform, false);

        float stripeLength = currentBounds.size.z * 0.78f;
        float stripeWidth = 0.10f;
        float gap = 0.06f;
        float roofY = currentBounds.max.y + 0.006f;
        float centerZ = currentBounds.center.z;

        Material stripeMat = CreateLitMaterial("Racing_Stripe_Vinyl", new Color(0.95f, 0.95f, 0.95f), 0.1f, 0.85f);

        for (int side = -1; side <= 1; side += 2)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = $"Stripe_{(side < 0 ? "L" : "R")}";
            stripe.transform.SetParent(stripesRoot.transform, false);
            float xPos = currentBounds.center.x + side * (gap * 0.5f + stripeWidth * 0.5f);
            stripe.transform.position = new Vector3(xPos, roofY, centerZ);
            stripe.transform.localScale = new Vector3(stripeWidth, 0.006f, stripeLength);
            stripe.GetComponent<Renderer>().sharedMaterial = stripeMat;
            RemoveCollider(stripe);
        }

        return stripesRoot;
    }

    private GameObject BuildUnderglow()
    {
        GameObject underglowRoot = new GameObject("Underglow_Lighting");
        underglowRoot.transform.SetParent(accessoryRoot.transform, false);

        float carWidth = currentBounds.size.x;
        float carLength = currentBounds.size.z;
        float groundY = platformTopY + 0.05f;
        float centerZ = currentBounds.center.z;
        float sideX = currentBounds.extents.x * 0.78f;

        Color neonColor = new Color(0f, 0.85f, 1f); // Vibrant Cyan Neon
        Material glowMat = CreateLitMaterial("Underglow_Neon", neonColor, 0f, 0.95f, neonColor * 2.5f);

        // Side neon tubes
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tube.name = $"NeonTube_{(side < 0 ? "L" : "R")}";
            tube.transform.SetParent(underglowRoot.transform, false);
            tube.transform.position = new Vector3(currentBounds.center.x + side * sideX, groundY, centerZ);
            tube.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            tube.transform.localScale = new Vector3(0.04f, carLength * 0.35f, 0.04f);
            tube.GetComponent<Renderer>().sharedMaterial = glowMat;
            RemoveCollider(tube);
        }

        // Front and Rear neon tubes
        float frontZ = currentBounds.center.z + currentBounds.extents.z * 0.65f;
        float backZ = currentBounds.center.z - currentBounds.extents.z * 0.65f;

        float[] endPositions = new float[] { frontZ, backZ };
        for (int i = 0; i < 2; i++)
        {
            GameObject crossTube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            crossTube.name = i == 0 ? "NeonTube_Front" : "NeonTube_Rear";
            crossTube.transform.SetParent(underglowRoot.transform, false);
            crossTube.transform.position = new Vector3(currentBounds.center.x, groundY, endPositions[i]);
            crossTube.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            crossTube.transform.localScale = new Vector3(0.04f, sideX * 0.8f, 0.04f);
            crossTube.GetComponent<Renderer>().sharedMaterial = glowMat;
            RemoveCollider(crossTube);
        }

        // Add soft downward point lights for rich ground illumination
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject lightObj = new GameObject($"Underglow_Light_{(side < 0 ? "L" : "R")}");
            lightObj.transform.SetParent(underglowRoot.transform, false);
            lightObj.transform.position = new Vector3(currentBounds.center.x + side * (sideX * 0.8f), groundY + 0.15f, centerZ);

            Light pLight = lightObj.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.range = 3.0f;
            pLight.intensity = 2.0f;
            pLight.color = neonColor;
            pLight.shadows = LightShadows.None;
        }

        return underglowRoot;
    }

    private void RemoveCollider(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }

    private void OnDestroy()
    {
        foreach (Material mat in createdMaterials)
        {
            if (mat != null) Destroy(mat);
        }
        createdMaterials.Clear();
    }
}

