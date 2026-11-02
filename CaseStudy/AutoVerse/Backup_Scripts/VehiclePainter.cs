
using System.Collections.Generic;
using UnityEngine;

public class VehiclePainter : MonoBehaviour
{
    public enum PaintFinish
    {
        Gloss,
        Metallic,
        Matte
    }

    [Header("References")]
    [SerializeField] private CarSelector carSelector;

    [Header("Paint Settings")]
    [SerializeField] private int defaultColorIndex = 0;

    private readonly Color[] paintColors =
    {
        new Color(1.00f, 0.35f, 0.08f), // Orange
        new Color(0.95f, 0.08f, 0.08f), // Red
        new Color(0.05f, 0.25f, 0.95f), // Blue
        new Color(0.02f, 0.65f, 0.35f), // Green
        new Color(0.95f, 0.80f, 0.10f), // Yellow
        new Color(0.04f, 0.04f, 0.05f), // Black
        new Color(0.95f, 0.95f, 0.95f), // White
        new Color(0.45f, 0.48f, 0.55f), // Silver
        new Color(0.35f, 0.10f, 0.60f), // Purple
        new Color(0.90f, 0.20f, 0.55f), // Pink
        new Color(0.10f, 0.70f, 0.85f), // Cyan
        new Color(0.55f, 0.25f, 0.10f), // Brown
        new Color(0.50f, 0.75f, 0.10f), // Lime
        new Color(0.80f, 0.55f, 0.20f), // Gold
        new Color(0.30f, 0.35f, 0.40f), // Graphite
        new Color(0.60f, 0.10f, 0.20f)  // Burgundy
    };

    private readonly Dictionary<GameObject, Material[]> originalMaterials =
        new Dictionary<GameObject, Material[]>();

    private readonly Dictionary<GameObject, Material[]> paintMaterials =
        new Dictionary<GameObject, Material[]>();

    private readonly Dictionary<GameObject, int> selectedColors =
        new Dictionary<GameObject, int>();

    private readonly Dictionary<GameObject, PaintFinish> selectedFinishes =
        new Dictionary<GameObject, PaintFinish>();

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

        SetupCurrentCar();
    }

    private void Update()
    {
        if (carSelector.CurrentCar == null)
            return;

        SetupCurrentCar();

        if (Input.GetKeyDown(KeyCode.C))
            NextColor();

        if (Input.GetKeyDown(KeyCode.M))
            SetFinish(PaintFinish.Metallic);

        if (Input.GetKeyDown(KeyCode.G))
            SetFinish(PaintFinish.Gloss);

        if (Input.GetKeyDown(KeyCode.N))
            SetFinish(PaintFinish.Matte);
    }

    private void SetupCurrentCar()
    {
        GameObject car = carSelector.CurrentCar;

        if (car == null || originalMaterials.ContainsKey(car))
            return;

        Renderer renderer = car.GetComponentInChildren<Renderer>();

        if (renderer == null)
            return;

        Material[] originals = renderer.sharedMaterials;
        Material[] copies = new Material[originals.Length];

        for (int i = 0; i < originals.Length; i++)
        {
            if (originals[i] != null)
                copies[i] = new Material(originals[i]);
        }

        originalMaterials[car] = originals;
        paintMaterials[car] = copies;
        selectedColors[car] = defaultColorIndex;
        selectedFinishes[car] = PaintFinish.Gloss;

        renderer.materials = copies;

        Debug.Log(
            "VehiclePainter initialized: " + car.name +
            " | Material slots: " + copies.Length
        );
    }

    public void NextColor()
    {
        GameObject car = carSelector.CurrentCar;

        if (car == null)
            return;

        SetupCurrentCar();

        selectedColors[car] =
            (selectedColors[car] + 1) % paintColors.Length;

        ApplyPaint(car);
    }

    public void SetColor(int colorIndex)
    {
        GameObject car = carSelector.CurrentCar;

        if (car == null)
            return;

        SetupCurrentCar();

        selectedColors[car] =
            Mathf.Clamp(colorIndex, 0, paintColors.Length - 1);

        ApplyPaint(car);
    }

    public void SetFinish(PaintFinish finish)
    {
        GameObject car = carSelector.CurrentCar;

        if (car == null)
            return;

        SetupCurrentCar();

        selectedFinishes[car] = finish;

        ApplyPaint(car);
    }

    private void ApplyPaint(GameObject car)
    {
        Renderer renderer = car.GetComponentInChildren<Renderer>();

        if (renderer == null)
            return;

        Material[] materials = paintMaterials[car];

        // Temporary: identify likely body material by name.
        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];

            if (mat == null)
                continue;

            string materialName = mat.name.ToLower();

            bool isBody =
                materialName.Contains("body") ||
                materialName.Contains("paint") ||
                materialName.Contains("car");

            if (!isBody)
                continue;

            Color color = paintColors[selectedColors[car]];

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.color = color;

            switch (selectedFinishes[car])
            {
                case PaintFinish.Gloss:
                    SetSurface(mat, 0f, 0.85f);
                    break;

                case PaintFinish.Metallic:
                    SetSurface(mat, 0.85f, 0.75f);
                    break;

                case PaintFinish.Matte:
                    SetSurface(mat, 0f, 0.12f);
                    break;
            }
        }

        renderer.materials = materials;

        Debug.Log(
            "Paint updated: " + car.name +
            " | Color: " + selectedColors[car] +
            " | Finish: " + selectedFinishes[car]
        );
    }

    private void SetSurface(
        Material material,
        float metallic,
        float smoothness)
    {
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);

        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);
    }

    private void OnDestroy()
    {
        foreach (Material[] materials in paintMaterials.Values)
        {
            foreach (Material material in materials)
            {
                if (material != null)
                    Destroy(material);
            }
        }
    }
}
