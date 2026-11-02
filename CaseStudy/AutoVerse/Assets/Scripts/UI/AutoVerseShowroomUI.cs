using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// AutoVerse - AutoVerseShowroomUI
/// Complete responsive automotive showroom interface.
/// Features a floating top navigation bar with clear vehicle switcher, action pills,
/// and a collapsible bottom customization dock with circular color swatches and expanded options.
/// Engineered for 100% legibility, responsive scaling across desktop and mobile,
/// and complete camera orbit protection when interacting with any UI element.
/// </summary>
public class AutoVerseShowroomUI : MonoBehaviour
{
    public static AutoVerseShowroomUI Instance { get; private set; }

    public enum CustomizationCategory
    {
        Paint,
        Wheels,
        Glass,
        Lights,
        Neon,
        Accessories,
        Camera,
        Studio
    }

    public enum DeviceLayoutMode
    {
        Desktop,
        Tablet,
        MobilePortrait,
        MobileLandscape
    }

    [Header("Managers")]
    [SerializeField] private CarSelector carSelector;
    [SerializeField] private VehiclePainter vehiclePainter;
    [SerializeField] private VehicleAccessoryManager accessoryManager;
    [SerializeField] private ShowroomCamera showroomCamera;
    [SerializeField] private ShowroomLightingManager lightingManager;

    [Header("Responsive Settings")]
    [SerializeField] private bool autoDetectLayout = true;
    [SerializeField] private DeviceLayoutMode activeLayout = DeviceLayoutMode.Desktop;

    private Canvas mainCanvas;
    private CanvasScaler canvasScaler;
    private RectTransform safeAreaRoot;
    private RectTransform topBarRoot;
    private RectTransform bottomDockRoot;
    private RectTransform optionsAreaRoot;
    private GameObject keyboardModalRoot;

    private CustomizationCategory currentCategory = CustomizationCategory.Paint;
    private bool isDockExpanded = true;
    private CustomizationCategory renderedCategory;
    private bool isKeyModalOpen = false;

    private Rect lastSafeArea;
    private readonly List<RaycastResult> pointerHits = new List<RaycastResult>();
    private PointerEventData pointerEvent;
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    // Header UI Elements
    private Text vehicleNameText;
    private Text vehicleCounterText;
    private Text lightingBtnText;
    private Text turntableBtnText;
    private Text dockToggleText;
    private Text activeSelectionLabel;

    private readonly List<Button> categoryTabButtons = new List<Button>();
    private readonly List<Image> categoryTabBgs = new List<Image>();

    // Visual Sprites & Font
    private Sprite roundedBoxSprite;
    private Sprite circleSprite;
    private Font standardFont;

    // Luxury Dark Automotive Color Palette
    private readonly Color colPanelBg = new Color(0.055f, 0.059f, 0.065f, 0.96f);
    private readonly Color colHeaderBg = new Color(0.045f, 0.049f, 0.055f, 0.97f);
    private readonly Color colCardBg = new Color(0.105f, 0.11f, 0.12f, 1f);
    private readonly Color colAccentCyan = new Color(0.89f, 0.77f, 0.56f, 1f);
    private readonly Color colAccentGold = new Color(0.94f, 0.84f, 0.65f, 1f);
    private readonly Color colBtnNormal = new Color(0.14f, 0.15f, 0.16f, 1f);
    private readonly Color colBtnHover = new Color(0.25f, 0.23f, 0.19f, 1f);
    private readonly Color colBtnActive = new Color(0.36f, 0.29f, 0.18f, 1f);
    private readonly Color colTextPrimary = new Color(0.98f, 0.99f, 1.00f, 1.0f);
    private readonly Color colTextMuted = new Color(0.73f, 0.74f, 0.75f, 1f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FindManagers();
        InitAssets();
        EnsureEventSystem();
        BuildCanvasHierarchy();
    }

    private void Start()
    {
        FindManagers();
        if (carSelector != null)
        {
            carSelector.OnCarChanged += HandleCarChanged;
        }

        UpdateLayout(true);
        UpdateHeaderInfo();
        RefreshCategoryTabs();
        RefreshOptionsContent();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        if (carSelector != null)
        {
            carSelector.OnCarChanged -= HandleCarChanged;
        }
    }

    private void FindManagers()
    {
        if (carSelector == null) carSelector = FindAnyObjectByType<CarSelector>();
        if (vehiclePainter == null) vehiclePainter = FindAnyObjectByType<VehiclePainter>();
        if (accessoryManager == null) accessoryManager = FindAnyObjectByType<VehicleAccessoryManager>();
        if (showroomCamera == null) showroomCamera = FindAnyObjectByType<ShowroomCamera>();
        if (lightingManager == null) lightingManager = FindAnyObjectByType<ShowroomLightingManager>();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight || Screen.safeArea != lastSafeArea)
        {
            UpdateLayout(false);
        }

        UpdateTurntableButtonText();
        UpdateLightingButtonText();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleDock();
        }

        if (Input.GetKeyDown(KeyCode.Question) || Input.GetKeyDown(KeyCode.Slash))
        {
            ToggleKeyboardModal();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isKeyModalOpen) ToggleKeyboardModal();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.A))
        {
            UpdateTurntableButtonText();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            UpdateLightingButtonText();
        }
    }

    private void HandleCarChanged(GameObject newCar)
    {
        UpdateHeaderInfo();
        RefreshOptionsContent();
    }

    /// <summary>
    /// Triple-check UI detection: Screen coordinates + RectTransform bounds + EventSystem GraphicRaycaster.
    /// Completely blocks click-through camera rotation when clicking buttons or color swatches.
    /// </summary>
    public bool IsPointerOverUI() => IsScreenPointOverUI(Input.mousePosition);

    public bool IsTouchOverUI(int fingerId)
    {
        for (int i = 0; i < Input.touchCount; i++)
            if (Input.GetTouch(i).fingerId == fingerId)
                return IsScreenPointOverUI(Input.GetTouch(i).position);
        return false;
    }

    private bool IsScreenPointOverUI(Vector2 position)
    {
        if (isKeyModalOpen) return true;
        if (topBarRoot != null && RectTransformUtility.RectangleContainsScreenPoint(topBarRoot, position)) return true;
        if (bottomDockRoot != null && RectTransformUtility.RectangleContainsScreenPoint(bottomDockRoot, position)) return true;
        if (EventSystem.current == null) return false;
        if (pointerEvent == null)
            pointerEvent = new PointerEventData(EventSystem.current);
        pointerEvent.position = position;
        pointerHits.Clear();
        EventSystem.current.RaycastAll(pointerEvent, pointerHits);
        return pointerHits.Count > 0;
    }

    #region Asset Initialization

    private void InitAssets()
    {
        standardFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (standardFont == null) standardFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        roundedBoxSprite = CreateRoundedBoxSprite(32, 32, 6);
        circleSprite = CreateAntialiasedCircleSprite(64);
    }

    private Sprite CreateRoundedBoxSprite(int width, int height, int radius)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] cols = new Color[width * height];
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            Vector2 q = new Vector2(Mathf.Abs(x + 0.5f - width * 0.5f) - (width * 0.5f - radius),
                                    Mathf.Abs(y + 0.5f - height * 0.5f) - (height * 0.5f - radius));
            float d = new Vector2(Mathf.Max(q.x, 0), Mathf.Max(q.y, 0)).magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0) - radius;
            cols[y * width + x] = new Color(1, 1, 1, Mathf.Clamp01(0.5f - d));
        }
        tex.SetPixels(cols);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
    }

    private Sprite CreateAntialiasedCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] cols = new Color[size * size];
        float radius = (size - 2) * 0.5f;
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = Mathf.Clamp01(radius + 0.5f - dist);
                cols[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        tex.SetPixels(cols);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }
    }

    #endregion

    #region Canvas Hierarchy & Responsive Anchoring

    private void BuildCanvasHierarchy()
    {
        GameObject canvasObj = new GameObject("AutoVerse_UI_Canvas");
        canvasObj.transform.SetParent(transform, false);

        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.pixelPerfect = true;

        canvasScaler = canvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Safe Area Root Container
        GameObject safeObj = new GameObject("SafeAreaRoot", typeof(RectTransform));
        safeObj.transform.SetParent(canvasObj.transform, false);
        safeAreaRoot = safeObj.GetComponent<RectTransform>();
        safeAreaRoot.anchorMin = Vector2.zero;
        safeAreaRoot.anchorMax = Vector2.one;
        safeAreaRoot.offsetMin = Vector2.zero;
        safeAreaRoot.offsetMax = Vector2.zero;

        BuildTopFloatingBar();
        BuildBottomCustomizationHub();
        BuildKeyboardModal();
    }

    private void UpdateLayout(bool force)
    {
        lastSafeArea = Screen.safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);

        if (autoDetectLayout)
        {
            if (aspect < 0.85f || Screen.width <= 540)
            {
                activeLayout = DeviceLayoutMode.MobilePortrait;
            }
            else if (aspect > 1.70f && Screen.height <= 520)
            {
                activeLayout = DeviceLayoutMode.MobileLandscape;
            }
            else if (aspect <= 1.35f || (Screen.width <= 1024 && Screen.height <= 1366 && aspect < 1.1f))
            {
                activeLayout = DeviceLayoutMode.Tablet;
            }
            else
            {
                activeLayout = DeviceLayoutMode.Desktop;
            }
        }

        Rect safe = Screen.safeArea;
        if (Screen.width > 0 && Screen.height > 0 && safe.width > 0 && safe.height > 0)
        {
            safeAreaRoot.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            safeAreaRoot.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
        }
        bool portrait = activeLayout == DeviceLayoutMode.MobilePortrait || activeLayout == DeviceLayoutMode.Tablet;
        canvasScaler.referenceResolution = portrait ? new Vector2(480, 800) : new Vector2(1280, 720);
        canvasScaler.matchWidthOrHeight = portrait ? 0f : 0.5f;
        Canvas.ForceUpdateCanvases();

        ApplyLayoutGeometry();
    }

    private void ApplyLayoutGeometry()
    {
        if (topBarRoot == null || bottomDockRoot == null) return;

        bool isPortrait = activeLayout == DeviceLayoutMode.MobilePortrait || activeLayout == DeviceLayoutMode.Tablet;

        // Top Bar Geometry: Anchored top-center with explicit height and margins
        topBarRoot.anchorMin = new Vector2(0f, 1f);
        topBarRoot.anchorMax = new Vector2(1f, 1f);
        topBarRoot.pivot = new Vector2(0.5f, 1f);

        float topMargin = isPortrait ? 8f : 12f;
        float topSideMargin = isPortrait ? 10f : 20f;
        float topHeight = isPortrait ? 164f : 70f;

        topBarRoot.anchoredPosition = new Vector2(0f, -topMargin);
        topBarRoot.sizeDelta = new Vector2(-topSideMargin * 2f, topHeight);

        // Bottom Dock Geometry: Anchored bottom-center with smooth collapsible height
        bottomDockRoot.anchorMin = new Vector2(0f, 0f);
        bottomDockRoot.anchorMax = new Vector2(1f, 0f);
        bottomDockRoot.pivot = new Vector2(0.5f, 0f);

        float dockMargin = isPortrait ? 8f : 12f;
        float dockSideMargin = isPortrait ? 8f : 24f;
        float dockHeight = isDockExpanded ? 236f : 52f;

        bottomDockRoot.anchoredPosition = new Vector2(0f, dockMargin);
        bottomDockRoot.sizeDelta = new Vector2(-dockSideMargin * 2f, dockHeight);

        if (dockToggleText != null)
        {
            dockToggleText.text = isDockExpanded ? "HIDE CUSTOMIZATION  /  Drag to orbit · Scroll or pinch to zoom" : "SHOW CUSTOMIZATION";
        }

        if (optionsAreaRoot != null)
        {
            optionsAreaRoot.gameObject.SetActive(isDockExpanded);
        }

        Transform tabs = bottomDockRoot.Find("CategoryTabsRowViewport");
        if (tabs != null) tabs.gameObject.SetActive(isDockExpanded);
        LayoutHeader(isPortrait);
        RectTransform card = keyboardModalRoot.transform.Find("ModalCard") as RectTransform;
        if (card != null) card.sizeDelta = new Vector2(Mathf.Min(620, safeAreaRoot.rect.width - 32), Mathf.Min(600, safeAreaRoot.rect.height - 32));
        RefreshOptionsContent();
    }

    private void LayoutHeader(bool portrait)
    {
        topBarRoot.GetComponent<HorizontalLayoutGroup>().enabled = false;
        foreach (Transform child in topBarRoot)
        {
            var layout = child.GetComponent<LayoutElement>();
            if (layout != null) layout.ignoreLayout = true;
            if (child.name == "Spacer") child.gameObject.SetActive(false);
        }
        PlaceHeader("BrandBadge", portrait ? new Vector2(0, 1) : new Vector2(0, 0.5f),
            portrait ? new Vector2(16, -8) : new Vector2(16, 0), new Vector2(180, 34));
        PlaceHeader("VehicleSwitcher", portrait ? new Vector2(0.5f, 1) : new Vector2(0.45f, 0.5f),
            portrait ? new Vector2(0, -46) : Vector2.zero, new Vector2(portrait ? 420 : 300, 48));
        PlaceHeader("ActionGroup", portrait ? new Vector2(0.5f, 1) : new Vector2(1, 0.5f),
            portrait ? new Vector2(0, -104) : new Vector2(-12, 0), new Vector2(portrait ? 420 : 390, 48));
    }

    private void PlaceHeader(string name, Vector2 anchor, Vector2 position, Vector2 size)
    {
        RectTransform rt = topBarRoot.Find(name) as RectTransform;
        if (rt == null) return;
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(anchor.x == 0 ? 0 : anchor.x == 1 ? 1 : 0.5f, anchor.y == 1 ? 1 : 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
    }

    #endregion

    #region Top Floating Bar Construction

    private void BuildTopFloatingBar()
    {
        GameObject barObj = new GameObject("TopFloatingBar", typeof(RectTransform), typeof(Image));
        barObj.transform.SetParent(safeAreaRoot, false);
        topBarRoot = barObj.GetComponent<RectTransform>();

        Image bg = barObj.GetComponent<Image>();
        bg.sprite = roundedBoxSprite;
        bg.type = Image.Type.Sliced;
        bg.color = colHeaderBg;

        // Horizontal Layout
        HorizontalLayoutGroup hlg = barObj.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(16, 16, 6, 6);
        hlg.spacing = 12;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;

        // 1. Left Brand Badge
        GameObject brandBadge = new GameObject("BrandBadge", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        brandBadge.transform.SetParent(barObj.transform, false);
        LayoutElement brandLe = brandBadge.GetComponent<LayoutElement>();
        brandLe.minWidth = 140;
        brandLe.preferredWidth = 160;

        HorizontalLayoutGroup brandHlg = brandBadge.GetComponent<HorizontalLayoutGroup>();
        brandHlg.spacing = 6;
        brandHlg.childAlignment = TextAnchor.MiddleLeft;
        brandHlg.childForceExpandWidth = false;
        brandHlg.childForceExpandHeight = true;

        Text brandText = CreateDirectText(brandBadge.transform, "AUTOVERSE", 16, FontStyle.Bold, colAccentCyan);
        brandText.alignment = TextAnchor.MiddleLeft;

        Text subText = CreateDirectText(brandBadge.transform, "ATELIER", 11, FontStyle.Normal, colTextMuted);
        subText.alignment = TextAnchor.MiddleLeft;

        // Spacer to push selector to center
        CreateFlexibleSpacer(barObj.transform);

        // 2. Center Vehicle Switcher Badge
        GameObject vehicleSwitcher = new GameObject("VehicleSwitcher", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        vehicleSwitcher.transform.SetParent(barObj.transform, false);

        Image vsBg = vehicleSwitcher.GetComponent<Image>();
        vsBg.sprite = roundedBoxSprite;
        vsBg.type = Image.Type.Sliced;
        vsBg.color = colCardBg;

        LayoutElement vsLe = vehicleSwitcher.GetComponent<LayoutElement>();
        vsLe.minWidth = 260;
        vsLe.preferredWidth = 320;
        vsLe.preferredHeight = 48;

        HorizontalLayoutGroup vsHlg = vehicleSwitcher.GetComponent<HorizontalLayoutGroup>();
        vsHlg.padding = new RectOffset(8, 8, 4, 4);
        vsHlg.spacing = 10;
        vsHlg.childAlignment = TextAnchor.MiddleCenter;
        vsHlg.childForceExpandWidth = false;
        vsHlg.childForceExpandHeight = true;

        // Left Arrow Button
        CreateCompactButton(vehicleSwitcher.transform, "<", 36, 36, () =>
        {
            carSelector?.PreviousCar();
            UpdateHeaderInfo();
            RefreshOptionsContent();
        }, colBtnNormal, colTextPrimary);

        // Vehicle Name & Index info column
        GameObject infoCol = new GameObject("InfoCol", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
        infoCol.transform.SetParent(vehicleSwitcher.transform, false);
        LayoutElement infoLe = infoCol.GetComponent<LayoutElement>();
        infoLe.minWidth = 160;
        infoLe.preferredWidth = 200;

        VerticalLayoutGroup infoVlg = infoCol.GetComponent<VerticalLayoutGroup>();
        infoVlg.spacing = 0;
        infoVlg.childAlignment = TextAnchor.MiddleCenter;
        infoVlg.childForceExpandWidth = true;
        infoVlg.childForceExpandHeight = false;

        vehicleNameText = CreateDirectText(infoCol.transform, "SPORTS COUPE", 14, FontStyle.Bold, Color.white);
        vehicleNameText.alignment = TextAnchor.MiddleCenter;

        vehicleCounterText = CreateDirectText(infoCol.transform, "VEHICLE 1 OF 8", 10, FontStyle.Bold, colAccentGold);
        vehicleCounterText.alignment = TextAnchor.MiddleCenter;

        // Right Arrow Button
        CreateCompactButton(vehicleSwitcher.transform, ">", 36, 36, () =>
        {
            carSelector?.NextCar();
            UpdateHeaderInfo();
            RefreshOptionsContent();
        }, colBtnNormal, colTextPrimary);

        // Spacer to balance layout
        CreateFlexibleSpacer(barObj.transform);

        // 3. Right Action Pills
        GameObject actionGroup = new GameObject("ActionGroup", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        actionGroup.transform.SetParent(barObj.transform, false);
        LayoutElement agLe = actionGroup.GetComponent<LayoutElement>();
        agLe.minWidth = 260;
        agLe.preferredWidth = 340;

        HorizontalLayoutGroup agHlg = actionGroup.GetComponent<HorizontalLayoutGroup>();
        agHlg.spacing = 8;
        agHlg.childAlignment = TextAnchor.MiddleRight;
        agHlg.childForceExpandWidth = false;
        agHlg.childForceExpandHeight = true;

        // Reset Pill
        CreateCompactButton(actionGroup.transform, "Reset", 72, 44, () =>
        {
            vehiclePainter?.ResetCustomization();
            accessoryManager?.ResetAccessories();
            RefreshOptionsContent();
        }, colBtnNormal, colAccentGold);

        // Turntable Toggle Pill
        GameObject ttBtn = CreateCompactButton(actionGroup.transform, "Spin: OFF", 88, 44, () =>
        {
            showroomCamera?.ToggleAutoRotate();
            UpdateTurntableButtonText();
        }, colBtnNormal, colAccentCyan);
        turntableBtnText = ttBtn.GetComponentInChildren<Text>();

        // Lighting Cycle Pill
        GameObject lightBtn = CreateCompactButton(actionGroup.transform, "Studio", 140, 44, () =>
        {
            if (lightingManager != null)
            {
                lightingManager.CycleLightingStyle();
                UpdateLightingButtonText();
                RefreshOptionsContent();
            }
        }, colBtnNormal, colTextPrimary);
        lightingBtnText = lightBtn.GetComponentInChildren<Text>();

        // Keyboard Guide Modal Pill
        CreateCompactButton(actionGroup.transform, "Help", 64, 44, () =>
        {
            ToggleKeyboardModal();
        }, colBtnNormal, colTextMuted);
    }

    #endregion

    #region Bottom Customization Hub

    private void BuildBottomCustomizationHub()
    {
        GameObject hubObj = new GameObject("BottomCustomizationHub", typeof(RectTransform), typeof(Image));
        hubObj.transform.SetParent(safeAreaRoot, false);
        bottomDockRoot = hubObj.GetComponent<RectTransform>();

        Image bg = hubObj.GetComponent<Image>();
        bg.sprite = roundedBoxSprite;
        bg.type = Image.Type.Sliced;
        bg.color = colPanelBg;

        VerticalLayoutGroup vlg = hubObj.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(14, 14, 8, 10);
        vlg.spacing = 6;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // 1. Collapse / Expand Bar
        GameObject collapseBar = new GameObject("CollapseBar", typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
        collapseBar.transform.SetParent(hubObj.transform, false);
        LayoutElement cbLe = collapseBar.GetComponent<LayoutElement>();
        cbLe.preferredHeight = 34;
        cbLe.minHeight = 34;
        collapseBar.GetComponent<Image>().color = new Color(1, 1, 1, 0.015f);

        dockToggleText = CreateDirectText(collapseBar.transform, "HIDE CUSTOMIZATION  /  Drag to orbit · Scroll or pinch to zoom", 10, FontStyle.Bold, colTextMuted);
        dockToggleText.alignment = TextAnchor.MiddleCenter;

        collapseBar.GetComponent<Button>().onClick.AddListener(ToggleDock);

        // 2. Category Navigation Tabs Bar
        GameObject tabsRow = CreateScrollableRow(hubObj.transform, "CategoryTabsRow", 46);

        categoryTabButtons.Clear();
        categoryTabBgs.Clear();

        BuildCategoryTab(tabsRow.transform, "Paint", CustomizationCategory.Paint);
        BuildCategoryTab(tabsRow.transform, "Wheels", CustomizationCategory.Wheels);
        BuildCategoryTab(tabsRow.transform, "Glass", CustomizationCategory.Glass);
        BuildCategoryTab(tabsRow.transform, "Lights", CustomizationCategory.Lights);
        BuildCategoryTab(tabsRow.transform, "Neon", CustomizationCategory.Neon);
        BuildCategoryTab(tabsRow.transform, "Accessories", CustomizationCategory.Accessories);
        BuildCategoryTab(tabsRow.transform, "Camera", CustomizationCategory.Camera);
        BuildCategoryTab(tabsRow.transform, "Studio", CustomizationCategory.Studio);

        // 3. Dynamic Options Area
        GameObject optionsObj = new GameObject("DynamicOptionsArea", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
        optionsObj.transform.SetParent(hubObj.transform, false);
        optionsAreaRoot = optionsObj.GetComponent<RectTransform>();

        LayoutElement optLe = optionsObj.GetComponent<LayoutElement>();
        optLe.minHeight = 90;
        optLe.preferredHeight = 105;
        optLe.flexibleHeight = 1f;

        VerticalLayoutGroup optVlg = optionsObj.GetComponent<VerticalLayoutGroup>();
        optVlg.spacing = 6;
        optVlg.childAlignment = TextAnchor.MiddleCenter;
        optVlg.childForceExpandWidth = true;
        optVlg.childForceExpandHeight = false;
    }

    private void BuildCategoryTab(Transform parent, string label, CustomizationCategory category)
    {
        GameObject tabObj = new GameObject($"Tab_{category}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        tabObj.transform.SetParent(parent, false);

        LayoutElement le = tabObj.GetComponent<LayoutElement>();
        le.minWidth = 96;
        le.preferredWidth = 110;
        le.preferredHeight = 40;

        Image img = tabObj.GetComponent<Image>();
        img.sprite = roundedBoxSprite;
        img.type = Image.Type.Sliced;
        img.color = currentCategory == category ? colBtnActive : colBtnNormal;

        categoryTabButtons.Add(tabObj.GetComponent<Button>());
        categoryTabBgs.Add(img);

        tabObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            currentCategory = category;
            RefreshCategoryTabs();
            RefreshOptionsContent();
        });

        Text t = CreateDirectText(tabObj.transform, label, 12, FontStyle.Bold, colTextPrimary);
        t.alignment = TextAnchor.MiddleCenter;
    }

    private void RefreshCategoryTabs()
    {
        for (int i = 0; i < categoryTabBgs.Count; i++)
        {
            CustomizationCategory cat = (CustomizationCategory)i;
            bool isCurrent = cat == currentCategory;
            categoryTabBgs[i].color = isCurrent ? colBtnActive : colBtnNormal;
        }
    }

    #endregion

    #region Dynamic Category Content Rendering

    private void RefreshOptionsContent()
    {
        if (optionsAreaRoot == null) return;
        ScrollRect[] oldScrolls = optionsAreaRoot.GetComponentsInChildren<ScrollRect>();
        var positions = new List<float>();
        if (renderedCategory == currentCategory)
            foreach (var scroll in oldScrolls) positions.Add(scroll.horizontalNormalizedPosition);
        renderedCategory = currentCategory;

        for (int i = optionsAreaRoot.childCount - 1; i >= 0; i--)
        {
            optionsAreaRoot.GetChild(i).gameObject.SetActive(false);
            Destroy(optionsAreaRoot.GetChild(i).gameObject);
        }

        switch (currentCategory)
        {
            case CustomizationCategory.Paint:
                RenderPaintOptions();
                break;
            case CustomizationCategory.Wheels:
                RenderWheelOptions();
                break;
            case CustomizationCategory.Glass:
                RenderGlassOptions();
                break;
            case CustomizationCategory.Lights:
                RenderLightOptions();
                break;
            case CustomizationCategory.Neon:
                RenderNeonOptions();
                break;
            case CustomizationCategory.Accessories:
                RenderAccessoryOptions();
                break;
            case CustomizationCategory.Camera:
                RenderCameraOptions();
                break;
            case CustomizationCategory.Studio:
                RenderStudioOptions();
                break;
        }
        Canvas.ForceUpdateCanvases();
        ScrollRect[] newScrolls = optionsAreaRoot.GetComponentsInChildren<ScrollRect>();
        for (int i = 0; i < Mathf.Min(positions.Count, newScrolls.Length); i++)
            newScrolls[i].horizontalNormalizedPosition = positions[i];
    }

    private void RenderPaintOptions()
    {
        if (vehiclePainter == null) return;

        // Row 1: Finishes & Active Color Label
        GameObject metaRow = CreateRow(optionsAreaRoot, 34);

        string[] finishes = vehiclePainter.GetFinishNames();
        int activeFinish = (int)vehiclePainter.CurrentFinish;
        for (int i = 0; i < finishes.Length; i++)
        {
            int idx = i;
            bool isActive = idx == activeFinish;
            CreateCompactButton(metaRow.transform, finishes[i], 80, 32, () =>
            {
                vehiclePainter.SetPaintFinish(idx);
                RefreshOptionsContent();
            }, isActive ? colBtnActive : colCardBg, isActive ? Color.white : colTextMuted);
        }

        CreateFlexibleSpacer(metaRow.transform);

        int curIdx = vehiclePainter.CurrentBodyColorIndex;
        string[] colorNames = vehiclePainter.GetBodyColorNames();
        string activeName = (curIdx >= 0 && curIdx < colorNames.Length) ? colorNames[curIdx] : "Factory";
        activeSelectionLabel = CreateDirectText(metaRow.transform, $"Color: {activeName} ({finishes[activeFinish]})", 12, FontStyle.Bold, colAccentCyan);
        activeSelectionLabel.alignment = TextAnchor.MiddleRight;

        // Row 2: Color Swatches with Horizontal Scroll
        Color[] colors = vehiclePainter.GetBodyColors();
        int activeColorIdx = vehiclePainter.CurrentBodyColorIndex;

        GameObject scrollObj = new GameObject("SwatchScroll", typeof(RectTransform), typeof(ScrollRect), typeof(LayoutElement), typeof(RectMask2D), typeof(Image));
        scrollObj.transform.SetParent(optionsAreaRoot, false);
        LayoutElement sLe = scrollObj.GetComponent<LayoutElement>();
        sLe.minHeight = 52;
        sLe.preferredHeight = 56;
        sLe.flexibleWidth = 1f;

        ScrollRect sr = scrollObj.GetComponent<ScrollRect>();
        scrollObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.005f);
        sr.viewport = scrollObj.GetComponent<RectTransform>();
        sr.horizontal = true;
        sr.vertical = false;
        sr.movementType = ScrollRect.MovementType.Clamped;

        GameObject content = new GameObject("Content", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(scrollObj.transform, false);
        sr.content = content.GetComponent<RectTransform>();
        sr.content.anchorMin = new Vector2(0, 0);
        sr.content.anchorMax = new Vector2(0, 1);
        sr.content.pivot = new Vector2(0, 0.5f);
        sr.content.sizeDelta = Vector2.zero;

        ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        HorizontalLayoutGroup cHlg = content.GetComponent<HorizontalLayoutGroup>();
        cHlg.spacing = 10;
        cHlg.padding = new RectOffset(6, 6, 2, 2);
        cHlg.childAlignment = TextAnchor.MiddleCenter;
        cHlg.childForceExpandWidth = false;
        cHlg.childForceExpandHeight = false;

        for (int i = 0; i < colors.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == activeColorIdx;
            CreateColorSwatch(content.transform, colors[i], isCurrent, () =>
            {
                vehiclePainter.SetBodyColor(idx);
                RefreshOptionsContent();
            });
        }
    }

    private void RenderWheelOptions()
    {
        if (vehiclePainter == null) return;

        // Row 1: Alloy Wheel Finishes
        GameObject row1 = CreateRow(optionsAreaRoot, 46);
        CreateDirectText(row1.transform, "Alloys:", 12, FontStyle.Bold, colAccentCyan);

        string[] names = vehiclePainter.GetWheelColorNames();
        int activeWheelIdx = vehiclePainter.CurrentWheelColorIndex;
        Color[] displayAlloys = new Color[]
        {
            new Color(0.12f, 0.12f, 0.12f),
            new Color(0.88f, 0.88f, 0.90f),
            new Color(0.32f, 0.34f, 0.36f),
            new Color(0.88f, 0.70f, 0.22f),
            new Color(0.60f, 0.42f, 0.24f)
        };

        for (int i = 0; i < names.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == activeWheelIdx;
            CreateCompactButton(row1.transform, names[idx], 90, 38, () =>
            {
                vehiclePainter.SetWheelColor(idx);
                RefreshOptionsContent();
            }, isCurrent ? colBtnActive : colCardBg, isCurrent ? Color.white : colTextMuted);
        }

        // Row 2: Brake Caliper Colors
        GameObject row2 = CreateRow(optionsAreaRoot, 46);
        CreateDirectText(row2.transform, "Calipers:", 12, FontStyle.Bold, colAccentGold);

        string[] calNames = vehiclePainter.GetCaliperColorNames();
        Color[] calColors = vehiclePainter.GetCaliperColors();
        int activeCalIdx = vehiclePainter.CurrentCaliperColorIndex;

        for (int i = 0; i < calNames.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == activeCalIdx;
            CreateColorSwatch(row2.transform, calColors[idx], isCurrent, () =>
            {
                vehiclePainter.SetCaliperColor(idx);
                RefreshOptionsContent();
            }, 36);
        }
    }

    private void RenderGlassOptions()
    {
        if (vehiclePainter == null) return;

        if (!vehiclePainter.SupportsGlass())
        {
            CreateDirectText(optionsAreaRoot, "Formula Race Car has an open cockpit (Window tinting not applicable).", 13, FontStyle.Bold, colTextMuted);
            return;
        }

        GameObject row = CreateRow(optionsAreaRoot, 50);
        string[] tints = vehiclePainter.GetWindowTintNames();
        int currentTint = vehiclePainter.CurrentWindowTintIndex;

        for (int i = 0; i < tints.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == currentTint;
            CreateCompactButton(row.transform, tints[i], 120, 42, () =>
            {
                vehiclePainter.SetWindowTint(idx);
                RefreshOptionsContent();
            }, isCurrent ? colBtnActive : colCardBg, isCurrent ? Color.white : colTextMuted);
        }
    }

    private void RenderLightOptions()
    {
        if (vehiclePainter == null) return;

        if (!vehiclePainter.SupportsHeadlights())
        {
            CreateDirectText(optionsAreaRoot, "Formula Race Car does not have headlight illumination units.", 13, FontStyle.Bold, colTextMuted);
            return;
        }

        GameObject row = CreateRow(optionsAreaRoot, 50);
        string[] styles = vehiclePainter.GetHeadlightStyleNames();
        int currentLight = vehiclePainter.CurrentHeadlightIndex;

        for (int i = 0; i < styles.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == currentLight;
            CreateCompactButton(row.transform, styles[idx], 130, 42, () =>
            {
                vehiclePainter.SetHeadlightStyle(idx);
                RefreshOptionsContent();
            }, isCurrent ? colBtnActive : colCardBg, isCurrent ? Color.white : colTextMuted);
        }
    }

    private void RenderNeonOptions()
    {
        if (vehiclePainter == null || accessoryManager == null) return;

        GameObject row1 = CreateRow(optionsAreaRoot, 46);
        bool isNeonOn = accessoryManager.IsAccessoryActive(VehicleAccessoryManager.ACC_UNDERGLOW);

        CreateCompactButton(row1.transform, isNeonOn ? "Underglow: [ON]" : "Underglow: [OFF]", 140, 40, () =>
        {
            accessoryManager.ToggleAccessory(VehicleAccessoryManager.ACC_UNDERGLOW);
            RefreshOptionsContent();
        }, isNeonOn ? colBtnActive : colCardBg, isNeonOn ? Color.white : colTextMuted);

        CreateDirectText(row1.transform, "Neon Palette:", 12, FontStyle.Bold, colAccentCyan);

        string[] neonNames = vehiclePainter.GetUnderglowColorNames();
        Color[] neonColors = vehiclePainter.GetUnderglowColors();
        int activeNeonIdx = vehiclePainter.CurrentUnderglowColorIndex;

        for (int i = 0; i < neonNames.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == activeNeonIdx;
            CreateColorSwatch(row1.transform, neonColors[idx], isCurrent, () =>
            {
                vehiclePainter.SetUnderglowColor(idx);
                RefreshOptionsContent();
            }, 36);
        }
    }

    private void RenderAccessoryOptions()
    {
        if (accessoryManager == null) return;

        GameObject row = CreateRow(optionsAreaRoot, 50);
        string[] names = accessoryManager.GetAccessoryNames();

        for (int i = 0; i < names.Length; i++)
        {
            int idx = i;
            bool isActive = accessoryManager.IsAccessoryActive(idx);
            string label = isActive ? $"[ON] {names[idx]}" : $"[OFF] {names[idx]}";

            CreateCompactButton(row.transform, label, 150, 42, () =>
            {
                accessoryManager.ToggleAccessory(idx);
                RefreshOptionsContent();
            }, isActive ? colBtnActive : colCardBg, isActive ? Color.white : colTextMuted);
        }

        CreateCompactButton(row.transform, "Clear All", 90, 42, () =>
        {
            accessoryManager.ResetAccessories();
            RefreshOptionsContent();
        }, colBtnNormal, colAccentGold);
    }

    private void RenderCameraOptions()
    {
        GameObject row = CreateRow(optionsAreaRoot, 46);

        CreateCompactButton(row.transform, "Hero 3/4", 95, 38, () => showroomCamera?.SetHeroView(), colCardBg);
        CreateCompactButton(row.transform, "Front", 80, 38, () => showroomCamera?.SetFrontView(), colCardBg);
        CreateCompactButton(row.transform, "Side", 80, 38, () => showroomCamera?.SetSideView(), colCardBg);
        CreateCompactButton(row.transform, "Rear", 80, 38, () => showroomCamera?.SetRearView(), colCardBg);
        CreateCompactButton(row.transform, "Top", 80, 38, () => showroomCamera?.SetTopView(), colCardBg);
        CreateCompactButton(row.transform, "Wheel", 85, 38, () => showroomCamera?.SetWheelView(), colCardBg);
        CreateCompactButton(row.transform, "Zoom +", 90, 44, () => showroomCamera?.ZoomIn(), colCardBg);
        CreateCompactButton(row.transform, "Zoom -", 90, 44, () => showroomCamera?.ZoomOut(), colCardBg);

        CreateFlexibleSpacer(row.transform);

        CreateCompactButton(row.transform, "Reset Angle", 110, 38, () => showroomCamera?.ResetView(), colBtnNormal, colAccentGold);
    }

    private void RenderStudioOptions()
    {
        if (lightingManager == null) return;

        GameObject row = CreateRow(optionsAreaRoot, 46);
        string[] styles = new string[] { "Luxury Studio", "Midnight Studio", "Bright Gallery", "Neon Garage" };
        int activeStyle = lightingManager.CurrentStyleIndex;

        for (int i = 0; i < styles.Length; i++)
        {
            int idx = i;
            bool isCurrent = idx == activeStyle;
            CreateCompactButton(row.transform, styles[idx], 140, 40, () =>
            {
                lightingManager.SetLightingStyle(idx);
                UpdateLightingButtonText();
                RefreshOptionsContent();
            }, isCurrent ? colBtnActive : colCardBg, isCurrent ? Color.white : colTextMuted);
        }

        CreateFlexibleSpacer(row.transform);

        CreateCompactButton(row.transform, "Brightness -", 100, 40, () =>
        {
            lightingManager.AdjustBrightness(-0.15f);
        }, colBtnNormal, colTextPrimary);

        CreateCompactButton(row.transform, "Brightness +", 100, 40, () =>
        {
            lightingManager.AdjustBrightness(0.15f);
        }, colBtnNormal, colTextPrimary);
    }

    #endregion

    #region Widget Helpers

    private GameObject CreateRow(Transform parent, float height)
    {
        return CreateScrollableRow(parent, "OptionRow", Mathf.Max(48, height));
    }

    private GameObject CreateScrollableRow(Transform parent, string name, float height)
    {
        var viewport = new GameObject(name + "Viewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect), typeof(LayoutElement));
        viewport.transform.SetParent(parent, false);
        viewport.GetComponent<Image>().color = new Color(1, 1, 1, 0.005f);
        var element = viewport.GetComponent<LayoutElement>();
        element.minHeight = element.preferredHeight = height;
        element.flexibleWidth = 1;
        var content = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        var rt = content.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = Vector2.zero;
        var group = content.GetComponent<HorizontalLayoutGroup>();
        group.spacing = 8;
        group.padding = new RectOffset(4, 4, 2, 2);
        group.childAlignment = TextAnchor.MiddleLeft;
        group.childForceExpandWidth = false;
        group.childForceExpandHeight = true;
        content.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        var scroll = viewport.GetComponent<ScrollRect>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.content = rt;
        scroll.horizontal = true;
        scroll.vertical = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.decelerationRate = 0.08f;
        scroll.scrollSensitivity = 24;
        return content;
    }

    private void CreateColorSwatch(Transform parent, Color color, bool isSelected, UnityEngine.Events.UnityAction onClick, float size = 44f)
    {
        size = Mathf.Max(44, size);
        GameObject swatchObj = new GameObject("Swatch", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        swatchObj.transform.SetParent(parent, false);

        LayoutElement le = swatchObj.GetComponent<LayoutElement>();
        le.minWidth = size;
        le.preferredWidth = size;
        le.minHeight = size;
        le.preferredHeight = size;

        Image img = swatchObj.GetComponent<Image>();
        img.sprite = circleSprite;
        img.color = color;

        Button btn = swatchObj.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
        ColorBlock feedback = btn.colors;
        feedback.highlightedColor = new Color(1.18f, 1.14f, 1.06f, 1);
        feedback.pressedColor = new Color(0.72f, 0.68f, 0.60f, 1);
        feedback.fadeDuration = 0.12f;
        btn.colors = feedback;

        // Active Ring Indicator
        if (isSelected)
        {
            GameObject ring = new GameObject("ActiveRing", typeof(RectTransform), typeof(Image));
            ring.transform.SetParent(swatchObj.transform, false);

            RectTransform rt = ring.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(-4, -4);
            rt.offsetMax = new Vector2(4, 4);

            Image ringImg = ring.GetComponent<Image>();
            ringImg.sprite = circleSprite;
            ringImg.color = colAccentGold;
            ringImg.raycastTarget = false;
            var fill = new GameObject("Paint", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(swatchObj.transform, false);
            var fillRt = fill.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;
            fill.GetComponent<Image>().sprite = circleSprite;
            fill.GetComponent<Image>().color = color;
            fill.GetComponent<Image>().raycastTarget = false;
            var dot = new GameObject("Selected", typeof(RectTransform), typeof(Image));
            dot.transform.SetParent(swatchObj.transform, false);
            dot.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 12);
            dot.GetComponent<Image>().sprite = circleSprite;
            dot.GetComponent<Image>().color = color.grayscale > 0.5f ? Color.black : Color.white;
            dot.GetComponent<Image>().raycastTarget = false;
            ring.transform.SetAsFirstSibling();
        }
    }

    private GameObject CreateCompactButton(Transform parent, string label, float width, float height, UnityEngine.Events.UnityAction onClick, Color? bgColor = null, Color? textColor = null)
    {
        GameObject btnObj = new GameObject($"Btn_{label}", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        btnObj.transform.SetParent(parent, false);

        LayoutElement le = btnObj.GetComponent<LayoutElement>();
        le.minWidth = width;
        le.preferredWidth = width;
        le.minHeight = Mathf.Max(44, height);
        le.preferredHeight = Mathf.Max(44, height);

        Image img = btnObj.GetComponent<Image>();
        img.sprite = roundedBoxSprite;
        img.type = Image.Type.Sliced;
        img.color = bgColor ?? colBtnNormal;

        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
        ColorBlock feedback = btn.colors;
        feedback.highlightedColor = new Color(1.18f, 1.14f, 1.06f, 1);
        feedback.pressedColor = new Color(0.72f, 0.68f, 0.60f, 1);
        feedback.fadeDuration = 0.12f;
        btn.colors = feedback;

        Text txt = CreateDirectText(btnObj.transform, label, 12, FontStyle.Bold, textColor ?? colTextPrimary);
        txt.alignment = TextAnchor.MiddleCenter;

        return btnObj;
    }

    private void CreateFlexibleSpacer(Transform parent)
    {
        GameObject spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
        spacer.transform.SetParent(parent, false);
        LayoutElement le = spacer.GetComponent<LayoutElement>();
        le.flexibleWidth = 1f;
    }

    private Text CreateDirectText(Transform parent, string content, int fontSize, FontStyle style, Color color)
    {
        GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(parent, false);

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Text txt = textObj.GetComponent<Text>();
        txt.text = content;
        txt.font = standardFont;
        txt.fontSize = Mathf.Max(13, fontSize);
        txt.raycastTarget = false;
        txt.alignment = TextAnchor.MiddleLeft;
        txt.fontStyle = style;
        txt.color = color;
        if (parent.GetComponent<LayoutGroup>() != null)
        {
            LayoutElement textLayout = textObj.AddComponent<LayoutElement>();
            textLayout.minHeight = txt.fontSize + 4;
            textLayout.preferredWidth = Mathf.Max(64, txt.preferredWidth + 8);
            textLayout.flexibleWidth = 1;
        }
        else
        {
            rt.offsetMin = new Vector2(6, 2);
            rt.offsetMax = new Vector2(-6, -2);
        }
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Truncate;
        return txt;
    }

    #endregion

    #region Toggles & Header Updates

    public void ToggleDock()
    {
        isDockExpanded = !isDockExpanded;
        ApplyLayoutGeometry();
    }

    public void ToggleKeyboardModal()
    {
        isKeyModalOpen = !isKeyModalOpen;
        if (keyboardModalRoot != null)
        {
            keyboardModalRoot.SetActive(isKeyModalOpen);
        }
    }

    private void UpdateHeaderInfo()
    {
        if (carSelector == null || carSelector.CurrentCar == null) return;

        string rawName = carSelector.CurrentCar.name;
        string displayName = GetVehicleDisplayName(rawName);

        if (vehicleNameText != null)
        {
            vehicleNameText.text = displayName;
        }

        if (vehicleCounterText != null)
        {
            vehicleCounterText.text = $"VEHICLE {carSelector.CurrentIndex + 1} OF {carSelector.CarCount}";
        }
    }

    private string GetVehicleDisplayName(string rawName)
    {
        string n = rawName.ToLower();
        if (n.Contains("sports")) return "SPORTS COUPE";
        if (n.Contains("sedan")) return "EXECUTIVE SEDAN";
        if (n.Contains("hatchback")) return "COMPACT HATCH";
        if (n.Contains("suv")) return "URBAN SUV";
        if (n.Contains("race")) return "GT FORMULA RACER";
        if (n.Contains("police")) return "POLICE INTERCEPTOR";
        if (n.Contains("taxi")) return "CITY TAXI";
        if (n.Contains("golf")) return "LUXURY GOLF CART";
        return rawName.Replace("car_", "").Replace("_", " ").ToUpper();
    }

    private void UpdateTurntableButtonText()
    {
        if (turntableBtnText != null && showroomCamera != null)
        {
            turntableBtnText.text = showroomCamera.IsAutoRotating ? "Spin: ON" : "Spin: OFF";
        }
    }

    private void UpdateLightingButtonText()
    {
        if (lightingBtnText != null && lightingManager != null)
        {
            lightingBtnText.text = lightingManager.CurrentStyleName;
        }
    }

    #endregion

    #region Keyboard Shortcuts Help Modal

    private void BuildKeyboardModal()
    {
        keyboardModalRoot = new GameObject("KeyboardShortcutsModal", typeof(RectTransform), typeof(Image));
        keyboardModalRoot.transform.SetParent(safeAreaRoot, false);

        RectTransform rt = keyboardModalRoot.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image bg = keyboardModalRoot.GetComponent<Image>();
        bg.color = new Color(0.02f, 0.03f, 0.05f, 0.85f);

        // Center Dialog Card
        GameObject card = new GameObject("ModalCard", typeof(RectTransform), typeof(Image));
        card.transform.SetParent(keyboardModalRoot.transform, false);

        RectTransform cardRt = card.GetComponent<RectTransform>();
        cardRt.anchorMin = new Vector2(0.5f, 0.5f);
        cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.pivot = new Vector2(0.5f, 0.5f);
        cardRt.sizeDelta = new Vector2(500, 480);

        Image cardBg = card.GetComponent<Image>();
        cardBg.sprite = roundedBoxSprite;
        cardBg.type = Image.Type.Sliced;
        cardBg.color = colCardBg;

        var helpViewport = new GameObject("HelpViewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
        helpViewport.transform.SetParent(card.transform, false);
        var helpRt = helpViewport.GetComponent<RectTransform>();
        helpRt.anchorMin = Vector2.zero;
        helpRt.anchorMax = Vector2.one;
        helpRt.offsetMin = new Vector2(8, 8);
        helpRt.offsetMax = new Vector2(-8, -8);
        helpViewport.GetComponent<Image>().color = new Color(1, 1, 1, 0.005f);
        var helpContent = new GameObject("HelpContent", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        helpContent.transform.SetParent(helpViewport.transform, false);
        var contentRt = helpContent.GetComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0, 1);
        contentRt.anchorMax = Vector2.one;
        contentRt.pivot = new Vector2(0.5f, 1);
        contentRt.sizeDelta = Vector2.zero;
        helpContent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var helpScroll = helpViewport.GetComponent<ScrollRect>();
        helpScroll.viewport = helpRt;
        helpScroll.content = contentRt;
        helpScroll.horizontal = false;
        helpScroll.movementType = ScrollRect.MovementType.Clamped;
        Transform helpParent = helpContent.transform;
        VerticalLayoutGroup vlg = helpContent.GetComponent<VerticalLayoutGroup>();
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(24, 24, 20, 20);
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.UpperCenter;

        Text title = CreateDirectText(helpParent, "YOUR SHOWROOM, YOUR WAY", 18, FontStyle.Bold, colAccentCyan);
        title.alignment = TextAnchor.MiddleCenter;

        CreateDirectText(helpParent, "Drag to explore. Pinch or scroll to zoom. Swipe the option rows to discover every finish and view.", 14, FontStyle.Normal, colTextMuted).GetComponent<LayoutElement>().minHeight = 64;
        string[,] shortcuts = new string[,]
        {
            { "0 - 7", "Direct Vehicle Selection (1 - 8)" },

            { "C", "Cycle Body Paint Color" },
            { "G / M / N / S", "Gloss / Metallic / Matte / Satin" },
            { "W / J / H", "Cycle Wheels / Glass Tint / Headlights" },
            { "K / P / I / U", "Toggle Light Bar / Spoiler / Stripes / Underglow" },
            { "F1 - F4", "Lighting Presets" },
            { "+ / -", "Adjust Showroom Brightness" },
            { "Space", "Reset Camera Angle" },
            { "A", "Toggle Auto Turntable" },
            { "X / Backspace", "Reset Customization" },
            { "Tab", "Toggle Control Dock" },
            { "? / /", "Toggle This Help Guide" }
        };

        for (int i = 0; i < shortcuts.GetLength(0); i++)
        {
            GameObject row = new GameObject("Row", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            row.transform.SetParent(helpParent, false);
            row.AddComponent<LayoutElement>().minHeight = 42;

            HorizontalLayoutGroup rowHlg = row.GetComponent<HorizontalLayoutGroup>();
            rowHlg.childForceExpandWidth = true;
            rowHlg.childForceExpandHeight = true;

            Text keyTxt = CreateDirectText(row.transform, shortcuts[i, 0], 12, FontStyle.Bold, colAccentGold);
            keyTxt.alignment = TextAnchor.MiddleLeft;

            Text descTxt = CreateDirectText(row.transform, shortcuts[i, 1], 12, FontStyle.Normal, colTextPrimary);
            descTxt.alignment = TextAnchor.MiddleRight;
        }

        // Close Button
        CreateCompactButton(helpParent, "Close (Esc)", 140, 36, () => ToggleKeyboardModal(), colBtnNormal, colTextPrimary);

        keyboardModalRoot.SetActive(false);
    }

    #endregion
}
