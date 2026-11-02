# AutoVerse — Verification & Testing Checklist

This document details both the automated batchmode test results and the manual Play Mode verification checklist for AutoVerse.

---

## 1. Automated Test Suite Execution

- **Engine**: Unity 6.6 (`6000.6.7f1`)
- **Pipeline**: Universal Render Pipeline (URP 17.x)
- **Suite**: `AutoVerseBuildPipeline.RunAutomatedTestSuite()`
- **Result**: **`68 / 68 PASSED (100%) — 0 FAILED`**

| Test Category | Description | Status |
| :--- | :--- | :--- |
| **Scene Resolution** | Load `AutoVerse_Showwroom.unity`, locate `AutoVerseManager`, `CarSelector`, `VehiclePainter`, `ShowroomLightingManager`, `ShowroomCamera` | **PASSED** |
| **Vehicle Selection** | Switch through all 8 vehicles (indices 0..7) and verify active GameObject status | **PASSED** (8/8) |
| **Body Colors** | Apply all 18 body colors across all 8 vehicles | **PASSED** (18/18) |
| **Finishes** | Apply Gloss, Metallic, Matte, and Satin finishes; verify `_Smoothness` and `_Metallic` URP values | **PASSED** (4/4) |
| **Wheel Colors** | Apply Black, Silver, Gunmetal, Gold, and Bronze to wheel material slots | **PASSED** (5/5) |
| **Window Tint** | Apply Clear, Light, Dark, and Limo tints to glass slots | **PASSED** (4/4) |
| **Headlights** | Apply White, Warm White, and Ice Blue emission/color | **PASSED** (3/3) |
| **Safety Guarding** | Verify `race_car` handles unsupported window tint and headlights gracefully without exceptions | **PASSED** |
| **Accessory Toggle** | Toggle Roof Light Bar, Rear Spoiler, Racing Stripes, and Underglow Lighting; verify dynamic bounds fitting | **PASSED** (4/4) |
| **Reset Customization** | Snapshot original vehicle materials and restore completely on reset | **PASSED** |
| **Lighting Styles** | Switch through Studio Neutral, Cyberpunk Neon, Golden Hour, and Midnight Luxury | **PASSED** (4/4) |
| **Camera Presets** | Trigger Hero, Front, Side, Rear, Top, and Wheel preset camera targets | **PASSED** (6/6) |
| **Turntable Mode** | Toggle showroom vehicle rotation on and off | **PASSED** |

---

## 2. Manual Play Mode Verification Checklist

Open the project in Unity Editor and press **Play** (`Ctrl + P`) to verify the following items:

### A. Responsive Screen Resolution & Safe Area
Test the UI across various aspect ratios using Unity's Game View resolution dropdown:

- [ ] **Desktop FHD (1920 × 1080)**:
  - [ ] Vehicle is centered and prominent in the 3D viewport.
  - [ ] Customization panel is docked on the right side.
  - [ ] Top bar displays vehicle selector, lighting switcher, turntable toggle, and reset.
  - [ ] Keyboard HUD is visible in bottom-left corner.
  - [ ] Collapse arrow cleanly folds the customization drawer to the right edge.
- [ ] **Laptop (1366 × 768)**:
  - [ ] All buttons and text scale adaptively without overlap or clipping.
  - [ ] Touch targets remain comfortably clickable.
- [ ] **Tablet Portrait (768 × 1024)**:
  - [ ] Layout adapts to tablet layout mode.
  - [ ] Swatches and category tabs remain easily reachable.
- [ ] **Mobile Portrait (390 × 844 / 360 × 800)**:
  - [ ] Customization panel transforms into a bottom-sheet drawer.
  - [ ] Category tabs scroll horizontally.
  - [ ] Swatch buttons have a minimum size of 44 × 44 pixels for easy tapping.
  - [ ] Keyboard shortcut HUD is automatically hidden.
  - [ ] Safe-area padding accommodates camera notch and home bar.
- [ ] **Mobile Landscape (844 × 390)**:
  - [ ] Customization panel docks neatly on the right.
  - [ ] Center viewport remains clear of UI obstruction.

---

### B. Vehicle Switching & Material Slots
Switch through all 8 vehicles using the top bar buttons or keys `1`–`8`:

- [ ] **`car_sports`**: Body paint (slot 0), glass (slot 1), wheels (slot 3), headlights (slot 4).
- [ ] **`car_sedan`**: Body paint (slot 0), glass (slot 1), wheels (slot 3), headlights (slot 4).
- [ ] **`car_hatchback`**: Body paint (slot 0), glass (slot 1), wheels (slot 3), headlights (slot 4).
- [ ] **`car_suv`**: Body paint (slot 0), glass (slot 1), wheels (slot 3), headlights (slot 4).
- [ ] **`race_car`**: Open cockpit; body paint (slot 0), livery (slots 1, 2), wheels (slot 5). Glass and headlights disabled notice shown.
- [ ] **`car_police`**: Body paint (slot 0), glass (slot 1), wheels (slot 4), headlights (slot 5), roof beacon preserved.
- [ ] **`car_taxi`**: Body paint (slot 0), glass (slot 1), wheels (slot 3), headlights (slot 4), taxi roof sign preserved.
- [ ] **`golf_cart`**: Body paint (slot 3), canopy (slot 0), glass (slot 4), wheels (slot 8), headlights (slot 9).

---

### C. Customization Controls
- [ ] **Body Paint**: Click each of the 18 color swatches. Verify car body changes immediately and smoothly.
- [ ] **Finishes**: Click Gloss, Metallic, Matte, and Satin. Check specular reflections under showroom lights.
- [ ] **Wheel Colors**: Select Black, Silver, Gunmetal, Gold, and Bronze. Confirm rim materials update.
- [ ] **Window Tint**: Select Clear, Light, Dark, and Limo. Confirm transparency changes.
- [ ] **Headlights**: Select White, Warm White, and Ice Blue. Confirm emission color updates.
- [ ] **Accessories**: Toggle Roof Light Bar, Rear Spoiler, Racing Stripes, and Underglow Lighting.
- [ ] **Reset**: Click Reset Customization (`R`). Car should instantly revert to its original default appearance.

---

### D. Lighting & Environment
Switch through the 4 lighting styles using the top bar or key `L`:
- [ ] **Studio Neutral**: Clean white 5500K studio lighting; balanced rim and fill lights.
- [ ] **Cyberpunk Neon**: High-contrast magenta key light, cyan rim light, moody architectural ambiance.
- [ ] **Golden Hour**: Warm 3200K sunset tones with soft directional shadows.
- [ ] **Midnight Luxury**: Sophisticated deep blue/violet key light with crisp silver highlights.
- [ ] **Showroom Decor**: Verify VIP entrance portal, wall accents, ceiling canopy, and turntable halo ring remain visible and do not clip camera movement.

---

### E. Camera Controls & Interaction Isolation
- [ ] **Left-Click / 1-Finger Drag**: Orbits the camera around the vehicle.
- [ ] **Right-Click / 2-Finger Drag**: Pans the camera horizontally and vertically.
- [ ] **Mouse Scroll Wheel / 2-Finger Pinch**: Zooms in and out smoothly (clamped between 3.0m and 9.5m).
- [ ] **UI Isolation**: Dragging or clicking on any button or swatch **does NOT** rotate or zoom the 3D car.
- [ ] **Camera Preset Buttons**:
  - [ ] **Hero**: 45° angle, balanced elevation.
  - [ ] **Front**: Direct front-facing bumper view.
  - [ ] **Side**: Direct profile view of wheels and body silhouette.
  - [ ] **Rear**: Direct view of rear bumper and spoiler.
  - [ ] **Top**: Elevated top-down view of roof and hood.
  - [ ] **Wheel**: Low-angle close-up focused on front-left wheel.
- [ ] **Turntable**: Toggle turntable mode (`Space`). Vehicle auto-rotates smoothly at 15°/sec.

