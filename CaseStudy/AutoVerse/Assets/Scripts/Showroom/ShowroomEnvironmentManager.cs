using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AutoVerse - ShowroomEnvironmentManager
/// Builds a luxury 360-degree automotive architectural showroom.
/// Features a polished obsidian reflective floor, ambient LED halo turntable,
/// modern architectural feature walls with backlit 3D AutoVerse emblem,
/// illuminated vertical light columns, showcase window bays, and overhead studio softbox canopy.
/// Automatically eliminates template floor spoke lights, tripods, and legacy dark void meshes.
/// </summary>
public class ShowroomEnvironmentManager : MonoBehaviour
{
    public static ShowroomEnvironmentManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ShowroomLightingManager lightingManager;

    [Header("Showroom Dimensions")]
    [SerializeField] private float roomHalfWidth = 14.0f;
    [SerializeField] private float roomHalfLength = 14.0f;
    [SerializeField] private float roomHeight = 7.8f;
    [SerializeField] private float turntableRadius = 4.5f;

    private GameObject environmentRoot;
    private readonly List<Material> decorMaterials = new List<Material>();

    private Material floorMat;
    private Material turntableMat;
    private Material wallMat;
    private Material featurePlaqueMat;
    private Material metalTrimMat;
    private Material emissiveAccentMat;
    private Material windowGlassMat;
    private Material stoneMat;
    private Material timberMat;
    private Material revealMat;

    private int lastLightingStyle = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (lightingManager == null)
            lightingManager = FindAnyObjectByType<ShowroomLightingManager>();

        CleanTemplateEnvironment();
        BuildShowroomArchitecture();

        if (lightingManager != null)
        {
            UpdateDecorLighting(lightingManager.CurrentStyleIndex);
        }
    }

    private void Update()
    {
        if (lightingManager != null)
        {
            int currentStyle = lightingManager.CurrentStyleIndex;
            if (currentStyle != lastLightingStyle)
            {
                lastLightingStyle = currentStyle;
                UpdateDecorLighting(currentStyle);
            }
        }
    }

    /// <summary>
    /// Eliminates template radial spoke sticks, tripods, and dark void backdrop planes.
    /// </summary>
    public void CleanTemplateEnvironment()
    {
        MeshRenderer[] renderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        foreach (MeshRenderer mr in renderers)
        {
            if (mr == null || mr.gameObject == null) continue;
            string n = mr.gameObject.name.ToLower();
            string pn = mr.transform.parent != null ? mr.transform.parent.name.ToLower() : "";

            if (n.Contains("border") || n.Contains("spotlight") || n.Contains("tripod") ||
                n.Contains("samplesspotlight") || n.Contains("backgroundmesh") ||
                pn.Contains("border") || pn.Contains("spotlight") || pn.Contains("tripod") ||
                pn.Contains("samplesspotlight"))
            {
                mr.enabled = false;
                mr.gameObject.SetActive(false);
            }
        }

        GameObject bgMesh = GameObject.Find("BackgroundMesh");
        if (bgMesh != null) bgMesh.SetActive(false);

        GameObject platformObj = GameObject.Find("Platform");
        if (platformObj != null)
        {
            foreach (Transform child in platformObj.transform)
            {
                if (child.name.Contains("Border") || child.name.Contains("SamplesSpotlight") || child.name.Contains("Tripod"))
                {
                    MeshRenderer r = child.GetComponent<MeshRenderer>();
                    if (r != null) r.enabled = false;
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    private Material CreateDecorMaterial(string name, Color baseColor, float metallic, float smoothness, Color? emission = null)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material mat = new Material(shader) { name = name };
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
        else if (mat.HasProperty("_Color")) mat.color = baseColor;

        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", emission.Value);
        }

        decorMaterials.Add(mat);
        return mat;
    }

    public void BuildShowroomArchitecture()
    {
        if (environmentRoot != null)
        {
            environmentRoot.SetActive(false);
            Destroy(environmentRoot);
        }

        environmentRoot = new GameObject("[AutoVerse_Luxury_Showroom]");
        environmentRoot.transform.SetParent(transform, false);

        foreach (Material material in decorMaterials)
            if (material != null) Destroy(material);
        decorMaterials.Clear();

        // Shared finishes keep the architectural details inexpensive to render.
        stoneMat = CreateDecorMaterial("Warm_Limestone", new Color(0.48f, 0.45f, 0.39f), 0.02f, 0.28f);
        timberMat = CreateDecorMaterial("Smoked_Walnut", new Color(0.18f, 0.105f, 0.065f), 0.02f, 0.32f);
        revealMat = CreateDecorMaterial("Recess_Shadow", new Color(0.035f, 0.04f, 0.045f), 0.05f, 0.3f);
        // Materials setup
        floorMat = CreateDecorMaterial("Floor_Obsidian_Polished", new Color(0.105f, 0.11f, 0.12f), 0.35f, 0.82f);
        turntableMat = CreateDecorMaterial("Turntable_Surface", new Color(0.17f, 0.18f, 0.19f), 0.50f, 0.78f);
        wallMat = CreateDecorMaterial("Wall_Architectural_Panels", new Color(0.24f, 0.23f, 0.22f), 0.15f, 0.50f);
        featurePlaqueMat = CreateDecorMaterial("Feature_Stage_Carbon", new Color(0.20f, 0.22f, 0.26f), 0.25f, 0.65f);
        metalTrimMat = CreateDecorMaterial("Trim_Brushed_Titanium", new Color(0.64f, 0.54f, 0.38f), 0.85f, 0.75f);
        emissiveAccentMat = CreateDecorMaterial("LED_Accent_Ambiance", new Color(1.0f, 0.85f, 0.50f), 0.1f, 0.9f, new Color(2.0f, 1.7f, 1.0f));
        windowGlassMat = CreateDecorMaterial("Showcase_Glass_Bay", new Color(0.20f, 0.25f, 0.32f, 0.85f), 0.90f, 0.95f);

        BuildPolishedFloor();
        BuildTurntableHalo();
        BuildPresentationBackdropWall();
        BuildSurroundingEnclosureWalls();
        BuildOverheadSoftboxCanopy();
        if (lightingManager != null) UpdateDecorLighting(lightingManager.CurrentStyleIndex);
    }

    #region Architectural Construction

    private void BuildPolishedFloor()
    {
        GameObject floorObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floorObj.name = "Showroom_Main_Floor";
        floorObj.transform.SetParent(environmentRoot.transform, false);
        floorObj.transform.position = new Vector3(0f, -0.005f, 0f);
        floorObj.transform.localScale = new Vector3(4.5f, 1.0f, 4.5f); // 45m x 45m
        floorObj.GetComponent<Renderer>().sharedMaterial = floorMat;
        RemoveCollider(floorObj);
    }

    private void BuildTurntableHalo()
    {
        GameObject ttRoot = new GameObject("Turntable_Platform");
        ttRoot.transform.SetParent(environmentRoot.transform, false);

        // Low profile circular turntable disc
        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Turntable_Disc";
        disc.transform.SetParent(ttRoot.transform, false);
        disc.transform.position = new Vector3(0f, 0.005f, 0f);
        disc.transform.localScale = new Vector3(turntableRadius * 2f, 0.01f, turntableRadius * 2f);
        disc.GetComponent<Renderer>().sharedMaterial = turntableMat;
        RemoveCollider(disc);

        // Dual Ambient LED halo rings flush with floor around turntable perimeter
        GameObject ringRoot = new GameObject("Turntable_LED_Halo");
        ringRoot.transform.SetParent(ttRoot.transform, false);

        int segments = 36;
        float segmentLength = (2f * Mathf.PI * (turntableRadius + 0.10f) / segments) * 0.98f;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * (360f / segments) * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * (turntableRadius + 0.10f);
            float z = Mathf.Cos(angle) * (turntableRadius + 0.10f);

            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.name = $"Halo_{i}";
            seg.transform.SetParent(ringRoot.transform, false);
            seg.transform.position = new Vector3(x, 0.015f, z);
            seg.transform.rotation = Quaternion.Euler(0f, (i * (360f / segments)) + 90f, 0f);
            seg.transform.localScale = new Vector3(segmentLength, 0.015f, 0.08f);
            seg.GetComponent<Renderer>().sharedMaterial = emissiveAccentMat;
            RemoveCollider(seg);
        }
    }

    private void BuildPresentationBackdropWall()
    {
        GameObject backWall = new GameObject("Presentation_Backdrop_Wall");
        backWall.transform.SetParent(environmentRoot.transform, false);

        float z = roomHalfLength;
        float width = roomHalfWidth * 2f;

        // Main Architectural Wall Slab
        GameObject mainSlab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mainSlab.name = "Back_Wall_Main";
        mainSlab.transform.SetParent(backWall.transform, false);
        mainSlab.transform.position = new Vector3(0f, roomHeight * 0.5f, z);
        mainSlab.transform.localScale = new Vector3(width, roomHeight, 0.4f);
        mainSlab.GetComponent<Renderer>().sharedMaterial = wallMat;
        RemoveCollider(mainSlab);

        // Center Feature Plaque (Elevated backdrop behind vehicle)
        GameObject featurePlaque = GameObject.CreatePrimitive(PrimitiveType.Cube);
        featurePlaque.name = "Center_Feature_Plaque";
        featurePlaque.transform.SetParent(backWall.transform, false);
        featurePlaque.transform.position = new Vector3(0f, 4.0f, z - 0.22f);
        featurePlaque.transform.localScale = new Vector3(12.0f, 5.0f, 0.18f);
        featurePlaque.GetComponent<Renderer>().sharedMaterial = featurePlaqueMat;
        RemoveCollider(featurePlaque);

        // Halo LED frame border around center plaque
        BuildFrameBorder(featurePlaque.transform, 12.0f, 5.0f, 0.09f, emissiveAccentMat);

        // Horizontal Architectural Reveal Trim Lines across the wall
        for (int i = 1; i <= 5; i++)
        {
            GameObject trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trim.name = $"Back_Trim_H{i}";
            trim.transform.SetParent(backWall.transform, false);
            trim.transform.position = new Vector3(0f, i * 1.35f, z - 0.22f);
            trim.transform.localScale = new Vector3(width - 2f, 0.03f, 0.06f);
            trim.GetComponent<Renderer>().sharedMaterial = metalTrimMat;
            RemoveCollider(trim);
        }

        // Two prominent flanking illuminated light pillars on the presentation stage
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = $"Stage_Light_Pillar_{(side < 0 ? "L" : "R")}";
            pillar.transform.SetParent(backWall.transform, false);
            pillar.transform.position = new Vector3(side * 6.5f, roomHeight * 0.5f, z - 0.35f);
            pillar.transform.localScale = new Vector3(0.5f, roomHeight, 0.4f);
            pillar.GetComponent<Renderer>().sharedMaterial = metalTrimMat;
            RemoveCollider(pillar);

            // Vertical glowing LED strip in the pillar
            GameObject pLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pLight.name = "Pillar_LED";
            pLight.transform.SetParent(pillar.transform, false);
            pLight.transform.localPosition = new Vector3(0f, 0f, -0.55f);
            pLight.transform.localScale = new Vector3(0.4f, 0.95f, 0.2f);
            pLight.GetComponent<Renderer>().sharedMaterial = emissiveAccentMat;
            RemoveCollider(pLight);
        }

        // Walnut fins frame the central stone display without crossing the brand panel.
        for (int side = -1; side <= 1; side += 2)
        for (int i = 0; i < 13; i++)
            CreateWallDetail(backWall.transform, "Walnut_Feature_Fin", new Vector3(side * (7.6f + i * 0.38f), roomHeight * 0.5f, z - 0.34f),
                new Vector3(0.16f, roomHeight - 0.65f, 0.23f), timberMat);
        CreateWallDetail(backWall.transform, "Stone_Feature_Plinth", new Vector3(0, 0.65f, z - 0.35f), new Vector3(12, 0.9f, 0.3f), stoneMat);

        // Backlit AUTOVERSE 3D Brand Logo Bar on Feature Wall
        BuildBrandLogoLettering(backWall.transform, new Vector3(0f, 4.4f, z - 0.35f));
    }

    private void BuildBrandLogoLettering(Transform parent, Vector3 centerPos)
    {
        GameObject logoRoot = new GameObject("AUTOVERSE_Brand_Emblem");
        logoRoot.transform.SetParent(parent, false);
        logoRoot.transform.position = centerPos;

        CreateWallDetail(logoRoot.transform, "Brand_Plaque", Vector3.zero, new Vector3(9.5f, 1.7f, 0.12f), revealMat);
        var lettering = new GameObject("AUTOVERSE_Lettering", typeof(TextMesh));
        lettering.transform.SetParent(logoRoot.transform, false);
        lettering.transform.localPosition = new Vector3(0, 0.14f, -0.08f);
        Font brandFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        TextMesh text = lettering.GetComponent<TextMesh>();
        text.font = brandFont;
        if (brandFont != null) lettering.GetComponent<MeshRenderer>().sharedMaterial = brandFont.material;
        text.text = "A U T O V E R S E";
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.fontSize = 96;
        text.characterSize = 0.095f;
        text.color = new Color(0.94f, 0.85f, 0.66f);
        var subtitle = new GameObject("Atelier_Subtitle", typeof(TextMesh));
        subtitle.transform.SetParent(logoRoot.transform, false);
        subtitle.transform.localPosition = new Vector3(0, -0.47f, -0.08f);
        TextMesh subText = subtitle.GetComponent<TextMesh>();
        subText.font = brandFont;
        if (brandFont != null) subtitle.GetComponent<MeshRenderer>().sharedMaterial = brandFont.material;
        subText.text = "THE AUTOMOTIVE ATELIER";
        subText.anchor = TextAnchor.MiddleCenter;
        subText.fontSize = 64;
        subText.characterSize = 0.045f;
        subText.color = new Color(0.76f, 0.74f, 0.68f);
    }

    private GameObject CreateWallDetail(Transform parent, string name, Vector3 position, Vector3 size, Material material)
    {
        GameObject detail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        detail.name = name;
        detail.transform.SetParent(parent, false);
        detail.transform.localPosition = position;
        detail.transform.localScale = size;
        var renderer = detail.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        RemoveCollider(detail);
        return detail;
    }

    private void BuildSurroundingEnclosureWalls()
    {
        GameObject sideWalls = new GameObject("Surrounding_Enclosure_Walls");
        sideWalls.transform.SetParent(environmentRoot.transform, false);

        // 1. Left Wall (x = -roomHalfWidth): Architectural Gallery Wall with LED Coves
        BuildArchitecturalSideWall(sideWalls.transform, -roomHalfWidth, true);

        // 2. Right Wall (x = +roomHalfWidth): Floor-to-ceiling Showcase Bay
        BuildShowcaseWindowWall(sideWalls.transform, roomHalfWidth);

        // 3. Front Wall Entrance (z = -roomHalfLength): Low-profile portal threshold
        GameObject frontWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontWall.name = "Front_Entrance_Portal";
        frontWall.transform.SetParent(sideWalls.transform, false);
        frontWall.transform.position = new Vector3(0f, roomHeight * 0.5f, -roomHalfLength);
        frontWall.transform.localScale = new Vector3(roomHalfWidth * 2f, roomHeight, 0.4f);
        frontWall.GetComponent<Renderer>().sharedMaterial = wallMat;
        RemoveCollider(frontWall);

        // A framed entrance composition gives the reverse orbit a finished backdrop.
        var entrance = new GameObject("Entrance_Stone_Portal");
        entrance.transform.SetParent(sideWalls.transform, false);
        entrance.transform.localPosition = new Vector3(0, 0, -roomHalfLength + 0.3f);
        CreateWallDetail(entrance.transform, "Recessed_Door_Pair", new Vector3(0, 2.65f, 0.05f), new Vector3(5.8f, 5.3f, 0.12f), windowGlassMat);
        for (int side = -1; side <= 1; side += 2)
        {
            CreateWallDetail(entrance.transform, "Limestone_Jamb", new Vector3(side * 3.3f, 3, 0.12f), new Vector3(0.65f, 6, 0.35f), stoneMat);
            CreateWallDetail(entrance.transform, "Door_Handle", new Vector3(side * 0.25f, 2.4f, 0.18f), new Vector3(0.035f, 0.85f, 0.08f), metalTrimMat);
        }
        CreateWallDetail(entrance.transform, "Portal_Lintel", new Vector3(0, 6.05f, 0.12f), new Vector3(7.25f, 0.5f, 0.35f), stoneMat);
        CreateWallDetail(entrance.transform, "Portal_Cove", new Vector3(0, 5.72f, 0.32f), new Vector3(5.9f, 0.035f, 0.035f), emissiveAccentMat);
        CreateWallDetail(entrance.transform, "Door_Center_Seam", new Vector3(0, 2.65f, 0.13f), new Vector3(0.04f, 5.3f, 0.03f), metalTrimMat);

        // Front wall horizontal guide light
        GameObject frontLED = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontLED.name = "Front_LED_Accent";
        frontLED.transform.SetParent(frontWall.transform, false);
        frontLED.transform.localPosition = new Vector3(0f, 0.15f, 0.55f);
        frontLED.transform.localScale = new Vector3(0.92f, 0.025f, 0.15f);
        frontLED.GetComponent<Renderer>().sharedMaterial = emissiveAccentMat;
        RemoveCollider(frontLED);
    }

    private void BuildArchitecturalSideWall(Transform parent, float posX, bool isLeft)
    {
        GameObject wall = new GameObject($"Side_Wall_{(isLeft ? "Left" : "Right")}");
        wall.transform.SetParent(parent, false);

        // Main Wall Slab
        GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slab.name = "Slab";
        slab.transform.SetParent(wall.transform, false);
        slab.transform.position = new Vector3(posX, roomHeight * 0.5f, 0f);
        slab.transform.localScale = new Vector3(0.4f, roomHeight, roomHalfLength * 2f);
        slab.GetComponent<Renderer>().sharedMaterial = wallMat;
        RemoveCollider(slab);

        float inward = isLeft ? 1f : -1f;
        for (int bay = 0; bay < 5; bay++)
        {
            float centerZ = (bay - 2) * (roomHalfLength * 0.37f);
            CreateWallDetail(wall.transform, "Stone_Gallery_Panel", new Vector3(posX + inward * 0.24f, roomHeight * 0.5f, centerZ),
                new Vector3(0.11f, roomHeight - 1.2f, roomHalfLength * 0.35f), bay % 2 == 0 ? stoneMat : featurePlaqueMat);
            CreateWallDetail(wall.transform, "Gallery_Brass_Reveal", new Vector3(posX + inward * 0.31f, 1.05f, centerZ),
                new Vector3(0.035f, 0.025f, roomHalfLength * 0.32f), metalTrimMat);
        }
        CreateWallDetail(wall.transform, "Continuous_Skirting", new Vector3(posX + inward * 0.3f, 0.22f, 0), new Vector3(0.15f, 0.38f, roomHalfLength * 2), revealMat);
        CreateWallDetail(wall.transform, "Ceiling_Cove_Ribbon", new Vector3(posX + inward * 0.38f, roomHeight - 0.3f, 0), new Vector3(0.06f, 0.05f, roomHalfLength * 1.95f), emissiveAccentMat);

        // Vertical Architectural Coves with LED light ribbons
        float[] covesZ = new float[] { -7f, -2.5f, 2.5f, 7f };
        for (int i = 0; i < covesZ.Length; i++)
        {
            float z = covesZ[i];
            float innerOffset = isLeft ? 0.35f : -0.35f;

            // Architectural column rib
            GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cube);
            col.name = $"Pillar_{i}";
            col.transform.SetParent(wall.transform, false);
            col.transform.position = new Vector3(posX + innerOffset, roomHeight * 0.5f, z);
            col.transform.localScale = new Vector3(0.35f, roomHeight, 0.85f);
            col.GetComponent<Renderer>().sharedMaterial = metalTrimMat;
            RemoveCollider(col);

            // Recessed vertical LED strip
            GameObject vLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vLight.name = $"Cove_LED_{i}";
            vLight.transform.SetParent(col.transform, false);
            vLight.transform.localPosition = new Vector3(isLeft ? 0.55f : -0.55f, 0f, 0f);
            vLight.transform.localScale = new Vector3(0.12f, 0.92f, 0.20f);
            vLight.GetComponent<Renderer>().sharedMaterial = emissiveAccentMat;
            RemoveCollider(vLight);
        }
    }

    private void BuildShowcaseWindowWall(Transform parent, float posX)
    {
        GameObject wall = new GameObject("Showcase_Window_Wall_Right");
        wall.transform.SetParent(parent, false);

        // Structural base and header frame
        GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slab.name = "Glass_Bay_Backing";
        slab.transform.SetParent(wall.transform, false);
        slab.transform.position = new Vector3(posX, roomHeight * 0.5f, 0f);
        slab.transform.localScale = new Vector3(0.4f, roomHeight, roomHalfLength * 2f);
        slab.GetComponent<Renderer>().sharedMaterial = windowGlassMat;
        RemoveCollider(slab);

        // Tall smoked-glass bays with slim brass mullions and recessed light ribbons.
        for (int i = -3; i <= 3; i++)
        {
            float z = i * roomHalfLength / 3.5f;
            CreateWallDetail(wall.transform, "Vertical_Brass_Mullion", new Vector3(posX - 0.28f, roomHeight * 0.5f, z), new Vector3(0.16f, roomHeight, 0.065f), metalTrimMat);
        }
        CreateWallDetail(wall.transform, "Window_Base_Stone", new Vector3(posX - 0.24f, 0.35f, 0), new Vector3(0.2f, 0.7f, roomHalfLength * 2), stoneMat);
        CreateWallDetail(wall.transform, "Window_Head_Cove", new Vector3(posX - 0.35f, roomHeight - 0.28f, 0), new Vector3(0.05f, 0.045f, roomHalfLength * 1.95f), emissiveAccentMat);

        // Architectural mullions with illuminated horizontal trims
        for (int i = 1; i <= 4; i++)
        {
            GameObject trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trim.name = $"Glass_Mullion_H{i}";
            trim.transform.SetParent(wall.transform, false);
            trim.transform.position = new Vector3(posX - 0.25f, i * 1.5f, 0f);
            trim.transform.localScale = new Vector3(0.15f, 0.05f, roomHalfLength * 1.9f);
            trim.GetComponent<Renderer>().sharedMaterial = metalTrimMat;
            RemoveCollider(trim);
        }
    }

    private void BuildOverheadSoftboxCanopy()
    {
        GameObject canopyRoot = new GameObject("Overhead_Studio_Softbox");
        canopyRoot.transform.SetParent(environmentRoot.transform, false);

        float canopyY = roomHeight - 0.45f;
        float sizeX = 9.0f;
        float sizeZ = 13.0f;

        // Structural suspension frame
        BuildFrameBorder(canopyRoot.transform, sizeX, sizeZ, 0.20f, metalTrimMat, canopyY);

        // Center Softbox Luminous Panel (Reflects gorgeous smooth specular highlights on car)
        GameObject softbox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        softbox.name = "Softbox_Luminous_Panel";
        softbox.transform.SetParent(canopyRoot.transform, false);
        softbox.transform.position = new Vector3(0f, canopyY, 0f);
        softbox.transform.localScale = new Vector3(sizeX * 0.90f, 0.06f, sizeZ * 0.90f);
        softbox.GetComponent<Renderer>().sharedMaterial = emissiveAccentMat;
        RemoveCollider(softbox);
    }

    private void BuildFrameBorder(Transform parent, float width, float height, float thickness, Material mat, float yPos = 0f)
    {
        var frame = new GameObject("Architectural_Frame");
        frame.transform.SetParent(environmentRoot.transform, false);
        bool overhead = yPos != 0f;
        frame.transform.position = overhead ? new Vector3(parent.position.x, yPos, parent.position.z) : parent.position + new Vector3(0, 0, -0.13f);
        for (int side = -1; side <= 1; side += 2)
        {
            CreateWallDetail(frame.transform, "Frame_Horizontal", overhead ? new Vector3(0, 0, side * height * 0.5f) : new Vector3(0, side * height * 0.5f, 0),
                new Vector3(width, thickness, thickness), mat);
            CreateWallDetail(frame.transform, "Frame_Vertical", new Vector3(side * width * 0.5f, 0, 0),
                overhead ? new Vector3(thickness, thickness, height) : new Vector3(thickness, height, thickness), mat);
        }
    }

    #endregion

    #region Lighting Presets & Material Sync

    public void UpdateDecorLighting(int styleIndex)
    {
        if (emissiveAccentMat == null || wallMat == null) return;

        Color accentColor;
        Color wallColor;
        Color floorColor;
        Color plaqueColor;

        switch (styleIndex)
        {
            case 0: // LuxuryStudio (Warm Gold & Champagne)
                accentColor = new Color(1.0f, 0.85f, 0.50f);
                wallColor = new Color(0.24f, 0.23f, 0.22f);
                floorColor = new Color(0.105f, 0.11f, 0.12f);
                plaqueColor = new Color(0.20f, 0.22f, 0.26f);
                break;

            case 1: // MidnightStudio (Ice Blue & Silver)
                accentColor = new Color(0.25f, 0.80f, 1.0f);
                wallColor = new Color(0.20f, 0.24f, 0.32f);
                floorColor = new Color(0.12f, 0.14f, 0.18f);
                plaqueColor = new Color(0.14f, 0.16f, 0.22f);
                break;

            case 2: // BrightGallery (Crisp Pure Daylight White)
                accentColor = new Color(0.96f, 0.98f, 1.0f);
                wallColor = new Color(0.82f, 0.85f, 0.90f);
                floorColor = new Color(0.65f, 0.68f, 0.72f);
                plaqueColor = new Color(0.75f, 0.78f, 0.84f);
                break;

            case 3: // NeonGarage (Cyberpunk Magenta / Cyan)
                accentColor = new Color(1.0f, 0.18f, 0.75f);
                wallColor = new Color(0.22f, 0.18f, 0.30f);
                floorColor = new Color(0.14f, 0.12f, 0.20f);
                plaqueColor = new Color(0.16f, 0.14f, 0.24f);
                break;

            default:
                accentColor = Color.white;
                wallColor = new Color(0.24f, 0.23f, 0.22f);
                floorColor = new Color(0.105f, 0.11f, 0.12f);
                plaqueColor = new Color(0.20f, 0.22f, 0.26f);
                break;
        }

        if (emissiveAccentMat.HasProperty("_BaseColor"))
            emissiveAccentMat.SetColor("_BaseColor", accentColor);
        if (emissiveAccentMat.HasProperty("_EmissionColor"))
            emissiveAccentMat.SetColor("_EmissionColor", accentColor * 1.8f);

        if (wallMat.HasProperty("_BaseColor"))
            wallMat.SetColor("_BaseColor", wallColor);

        if (featurePlaqueMat != null && featurePlaqueMat.HasProperty("_BaseColor"))
            featurePlaqueMat.SetColor("_BaseColor", plaqueColor);

        if (floorMat != null && floorMat.HasProperty("_BaseColor"))
            floorMat.SetColor("_BaseColor", floorColor);
    }

    private void RemoveCollider(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        foreach (Material mat in decorMaterials)
        {
            if (mat != null) Destroy(mat);
        }
        decorMaterials.Clear();
    }

    #endregion
}
