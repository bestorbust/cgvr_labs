# AutoVerse — 3D Interactive Automotive Showroom & Customizer

[![Live WebGL Demo](https://img.shields.io/badge/Live_Demo-auto--verse--plum.vercel.app-00E5FF?style=for-the-badge&logo=vercel)](https://auto-verse-plum.vercel.app/)
[![Unity 6 Version](https://img.shields.io/badge/Unity_6-6000.6.7f1-000000?style=for-the-badge&logo=unity)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Pipeline-Universal_Render_Pipeline_(URP)-4B5563?style=for-the-badge)](https://unity.com/srp/universal-render-pipeline)
[![Target Platform](https://img.shields.io/badge/Platform-WebGL_2.0_|_Windows_Standalone-059669?style=for-the-badge)](https://auto-verse-plum.vercel.app/)

> **Course**: Computer Graphics & Virtual Reality (CGVR) — Semester 7  
> **Case Study**: Interactive 3D Real-Time Vehicle Showroom & Customizer  
> **Live Production URL**: [https://auto-verse-plum.vercel.app/](https://auto-verse-plum.vercel.app/)

---

## 1. Executive Summary

**AutoVerse** is a production-grade, interactive 3D automotive showroom and vehicle customization system developed in **Unity 6.6 (`6000.6.7f1`)** using the **Universal Render Pipeline (URP)** and C#.

The application provides an immersive, high-fidelity digital showroom experience inspired by luxury automotive design studios (e.g., Porsche, Mercedes-Benz, Tesla). It features:
- **8 interactive vehicle models** with real-time switching and custom mesh topologies.
- **Deep multi-layer customization**: 18 body paint colors across 4 distinct surface finishes (Gloss, Metallic, Matte, Satin), 5 alloy finishes, 5 Brembo brake caliper colors, 6 window tint styles, 5 LED headlight styles, and 6 underglow neon colors with real-time ground bounce lighting.
- **Procedural bounds-fitted accessories**: Roof light bars, rear wings/spoilers, racing stripes, and neon chassis lighting.
- **360° Architectural Luxury Showroom**: Polished obsidian reflective floor, illuminated turntable with dual ambient LED halo rings, acoustic feature wall with backlit 3D **AUTOVERSE** branding emblem, vertical architectural light columns, showcase window bay, and suspended overhead studio softbox canopy.
- **4 Real-time Studio Lighting Presets**: Luxury Studio (warm champagne & tungsten), Midnight Studio (high-tech ice blue & cyan rim), Bright Gallery (pure daylight white high-key pavilion), and Neon Garage (cyberpunk magenta & electric cyan).
- **Responsive & Touch-Optimized UI**: Dynamic CanvasScaler with safe-area handling across Desktop, Tablet, Mobile Portrait, and Mobile Landscape with 100% camera orbit isolation.

---

## 2. Live Deployment

The WebGL build is deployed with Brotli decompression and HTTPS static hosting on Vercel:

| Environment | URL | Status |
| :--- | :--- | :---: |
| **Production WebGL** | [https://auto-verse-plum.vercel.app/](https://auto-verse-plum.vercel.app/) | Active |
| **Local WebGL Test Server** | `http://localhost:8000/` (via `Run_WebGL_Server.bat`) | Ready |
| **Windows Standalone** | `AutoVerse/Builds/AutoVerse.exe` | Ready |

---

## 3. Vehicle Topology & Compatibility Matrix

Every vehicle model features unique sub-mesh slots, materials, and specialized customization handling:

| Index | Vehicle Identifier | Display Title | Body Paint Slot | Glass Tint Slot | Wheel Rim Slot | Headlights Slot | Specialized Custom Components |
| :---: | :--- | :--- | :---: | :---: | :---: | :---: | :--- |
| `0` | `car_sports` | **SPORTS COUPE** | Slot 0 (`orange`) | Slot 1 (`glass`) | Slot 3 (`hub`) | Slot 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) |
| `1` | `car_sedan` | **EXECUTIVE SEDAN** | Slot 0 (`carblue`) | Slot 1 (`glass`) | Slot 3 (`hub`) | Slot 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) |
| `2` | `car_hatchback` | **COMPACT HATCH** | Slot 0 (`carred`) | Slot 1 (`glass`) | Slot 3 (`hub`) | Slot 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) |
| `3` | `car_suv` | **URBAN SUV** | Slot 0 (`cargreen`) | Slot 1 (`glass`) | Slot 3 (`hub`) | Slot 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) |
| `4` | `race_car` | **GT FORMULA RACER** | Slot 0 (`carred`) | *N/A (Open)* | Slot 5 (`hub`) | *N/A (Track)* | Slots 1 & 2 (`aero`), Slot 3 (`monocoque`) |
| `5` | `car_police` | **POLICE INTERCEPTOR** | Slot 0 (`white`) | Slot 1 (`glass`) | Slot 4 (`hub`) | Slot 5 (`headlight`) | Slot 2 (`navy`), Slot 8 (`emergency light bar`) |
| `6` | `car_taxi` | **CITY TAXI** | Slot 0 (`taxi`) | Slot 1 (`glass`) | Slot 3 (`hub`) | Slot 4 (`headlight`) | Slot 2 (`interior`), Slot 7 (`roof sign`) |
| `7` | `golf_cart` | **LUXURY GOLF CART** | Slot 3 (`lime`) | Slot 4 (`glass`) | Slot 8 (`hub`) | Slot 9 (`headlight`) | Slot 0 (`canopy`), Slot 1 (`seats`), Slot 2 (`chassis`) |

---

## 4. Customization Suite

### A. Body Paint & Surface Finishes
* **18 Automotive Colors**:
  1. Pearl White (`#F5F5FA`)
  2. Midnight Black (`#141417`)
  3. Graphite (`#40454D`)
  4. Arctic Silver (`#D1D6DC`)
  5. Racing Red (`#D91F1F`)
  6. Crimson (`#8C0017`)
  7. Sunset Orange (`#FF5C1F`)
  8. Solar Yellow (`#FFCC00`)
  9. Royal Blue (`#144DCC`)
  10. Sky Blue (`#38B8FF`)
  11. Emerald Green (`#00A666`)
  12. Forest Green (`#1F522E`)
  13. Violet (`#7314B8`)
  14. Rose Gold (`#BF7380`)
  15. Champagne (`#F7E8CF`)
  16. Bronze (`#946133`)
  17. Teal (`#008C8C`)
  18. Candy Pink (`#F22673`)
* **4 Surface Finishes**:
  - **Gloss**: High smoothness ($0.90$), low metallic ($0.08$) with brilliant clearcoat reflections.
  - **Metallic**: High metallic ($0.90$), balanced micro-roughness ($0.85$) with deep flake luster.
  - **Matte**: Zero metallic ($0.00$), low smoothness ($0.15$) for stealth diffused absorption.
  - **Satin**: Mid metallic ($0.45$), velvety semi-specular sheen ($0.50$).

### B. Wheels & Calipers
* **5 Alloy Wheel Finishes**: Stealth Black, Polished Silver, Gunmetal, Liquid Gold, Burnt Bronze.
* **5 Brake Caliper Colors**: Brembo Racing Red, Acid Yellow, Electric Blue, Acid Lime Green, Stealth Gloss Black.

### C. Glass Tinting
* **6 Tint Styles**: Clear Glass ($35\%$), Light Smoke ($55\%$), Dark Charcoal ($82\%$), Limo Blackout ($96\%$), Sunset Gold Chameleon, Cyber Blue Mirror.

### D. Illumination & Neon Underglow
* **5 Headlight Styles**: Xenon White ($6500\text{K}$), Warm Halogen ($3200\text{K}$), Ice Blue LED ($8500\text{K}$), Cyber Violet LED, Amber Gold DRL.
* **6 Underglow Neon Colors**: Electric Cyan, Neon Magenta, Acid Green, Amber Gold, Ice White, Royal Violet with downward ground-illuminating point lights.

### E. Procedural Accessories
* **Roof Light Bar**: Dual high-intensity auxiliary off-road driving beam housing.
* **Rear Wing / Spoiler**: Aerodynamic downforce spoiler automatically scaled and positioned on vehicle rear deck lid.
* **Dual Racing Stripes**: Twin center racing livery stripes dynamically projected along hood, roof, and trunk.
* **Underglow Tubes**: Quad perimeter neon chassis tubes with real-time ground reflection.

---

## 5. Architectural Showroom & Lighting Rig

```
                        [ Ceiling Softbox Luminaire (9m x 13m) ]
                                      |
       [ Side Wall Gallery ]          v           [ Showcase Window Bay ]
      (4x LED Light Columns)    +-----------+     (Floor-to-Ceiling Glass)
                                |  VEHICLE  |
      [ Studio Key Light ] ---> |  TURNTABLE| <--- [ Studio Fill Light ]
        (45° Pitch, -35° Yaw)   +-----------+        (35° Pitch, 145° Yaw)
                                      ^
                                      |
                     [ Polished Obsidian Floor (45m x 45m) ]
                     [ Dual Ambient LED Perimeter Halo Ring ]
```

* **Elimination of Legacy Fixtures**: All template radial spoke sticks (`Border`), tripod spotlights (`SamplesSpotlight`, `Tripod_Neck`, `Spotlight_Head`), and dark cyc mesh (`BackgroundMesh`) are automatically purged from rendering buffers at startup.
* **4 Synchronized Studio Styles**:
  1. **Luxury Studio**: Warm champagne key ($2.1$), cool sky fill ($1.25$), soft tungsten accents, dark slate studio walls (`#212630`).
  2. **Midnight Studio**: High-tech ice blue key ($1.9$), electric cyan rim ($1.15$), deep midnight navy walls (`#141A24`).
  3. **Bright Gallery**: Pure daylight white high-key pavilion ($2.4$), crisp ambient bounce ($0.68$), modern gallery grey (`#B8C0CC`).
  4. **Neon Garage**: Cyberpunk magenta key ($2.0$), electric cyan rim ($1.3$), dark violet atmosphere (`#1A1426`).

---

## 6. Controls Reference

### Mouse & Touch Interaction
* **Orbit Camera**: Left-Click & Drag (Desktop) or 1-Finger Drag (Touch). Minimum 10px intentional drag deadzone prevents accidental camera rotation when clicking buttons.
* **Zoom View**: Mouse Scroll Wheel (Desktop) or 2-Finger Pinch In/Out (Touch).
* **Reset Camera**: Single click on **"Reset Angle"** button or press `Space`.
* **Turntable**: Click **"Spin: ON/OFF"** button or press `A` (15°/sec continuous rotation without camera angle jumps).

### Keyboard Shortcuts
| Key | Action |
| :--- | :--- |
| `0` – `7` | Direct Vehicle Selection (`SPORTS`, `SEDAN`, `HATCH`, `SUV`, `RACER`, `POLICE`, `TAXI`, `GOLF CART`) |
| `Left` / `Right Arrow` | Previous / Next Vehicle |
| `C` | Cycle Body Paint Color (18 colors) |
| `G` / `M` / `N` / `S` | Select Finish: Gloss / Metallic / Matte / Satin |
| `W` | Cycle Alloy Wheel Finish (5 styles) |
| `J` | Cycle Window Tint (6 styles) |
| `H` | Cycle Headlight Style (5 styles) |
| `K` | Toggle Roof Light Bar |
| `P` | Toggle Rear Spoiler / Wing |
| `I` | Toggle Dual Racing Stripes |
| `U` | Toggle Underglow Neon Lighting |
| `1` – `4` / `F1` – `F4` | Select Lighting Preset (Luxury / Midnight / Gallery / Neon) |
| `+` / `-` | Increase / Decrease Showroom Master Brightness |
| `Space` | Reset Camera to Hero 3/4 View |
| `A` | Toggle Turntable Auto-Rotation |
| `X` / `Backspace` | Reset Current Vehicle to Factory Customization |
| `Tab` | Toggle Collapsible Customization Dock |
| `?` or `/` | Toggle Keyboard Shortcuts Help Modal |

---

## 7. Directory Structure

```
CaseStudy/
├── README.md                           # This case study overview document
├── .gitignore                          # Root ignore rules for Unity & WebGL artifacts
├── .gitattributes                     # Git LFS & line endings configuration
└── AutoVerse/                          # Unity Project Root
    ├── Assets/
    │   ├── Scenes/
    │   │   └── AutoVerse_Showwroom.unity  # Primary showroom scene
    │   └── Scripts/
    │       ├── Core/
    │       │   └── CarSelector.cs         # Vehicle switcher & lifecycle management
    │       ├── Customization/
    │       │   ├── VehiclePainter.cs      # Paint, finishes, wheels, calipers, tints, headlights
    │       │   └── VehicleAccessoryManager.cs # Bounds-fitted spoilers, lightbars, stripes, neon
    │       ├── Camera/
    │       │   └── ShowroomCamera.cs      # Orbital camera, pinch zoom, UI click isolation
    │       ├── Showroom/
    │       │   ├── ShowroomLightingManager.cs # Studio key/fill/softbox rig & 4 lighting styles
    │       │   └── ShowroomEnvironmentManager.cs # 360° architectural walls, floor, halo turntable
    │       ├── UI/
    │       │   └── AutoVerseShowroomUI.cs # Responsive top floating bar & customization dock
    │       └── Editor/
    │           └── AutoVerseBuildPipeline.cs # Automated validation tests & build pipelines
    ├── Builds/
    │   ├── WEB-GL/                        # Production WebGL distribution
    │   └── Windows/                       # Windows x64 standalone executable
    ├── ProjectSettings/                   # URP graphics, quality, input, and player configurations
    ├── Packages/                          # Unity package manifest (URP, UI, Input System)
    ├── Run_WebGL_Server.bat               # Local Python Brotli HTTP test server
    ├── vercel.json                        # Vercel MIME-types, compression, & caching config
    └── README.md                          # Detailed AutoVerse engine documentation
```

---

## 8. Development & Build Instructions

### Prerequisites
* **Unity Version**: Unity 6.6 (`6000.6.7f1`) or newer with WebGL Build Support and Windows Build Support modules installed.
* **Render Pipeline**: Universal Render Pipeline (URP).
* **Local Web Server** (for WebGL testing): Python 3.8+ (for Brotli decompression support).

### Opening the Project
1. Launch **Unity Hub**.
2. Click **Add > Add project from disk**.
3. Select `CaseStudy/AutoVerse`.
4. Open the project using Unity Editor `6000.6.7f1`.
5. In the Project tab, navigate to `Assets/Scenes/AutoVerse_Showwroom.unity` and double-click to open.
6. Press the **Play** button in the top toolbar.

### Running Automated Test Suite
From the top menu in Unity Editor:
* Click **AutoVerse > Run All Validation Tests**.
* The test runner executes 37 automated assertions covering:
  - 8-vehicle loading and hierarchy activation.
  - 18 body color presets and 4 finishes.
  - 5 wheel finishes and 5 caliper colors.
  - Window tinting and headlight styles on supported vs. guarded vehicles (e.g. formula open cockpit).
  - 4 procedural accessories toggle states.
  - Camera presets and studio lighting styles.

### Generating WebGL Build
1. In Unity Editor, go to **File > Build Settings** (or **Build Profiles** in Unity 6).
2. Switch platform to **Web**.
3. Ensure `Assets/Scenes/AutoVerse_Showwroom.unity` is checked.
4. Player Settings verification:
   - **Compression Format**: Brotli.
   - **Publishing Settings > Decompression Fallback**: Enabled.
   - **Color Space**: Linear.
5. Click **Build** and choose output directory: `CaseStudy/AutoVerse/Builds/WEB-GL/`.
6. To test locally: double-click `Run_WebGL_Server.bat` and open `http://localhost:8000/`.

---

## 9. Technology Stack

* **Game Engine**: Unity 6.6 (`6000.6.7f1`)
* **Graphics API**: WebGL 2.0 / DirectX 11 / Metal
* **Render Pipeline**: Universal Render Pipeline (URP Forward Renderer)
* **Scripting**: C# (.NET Standard 2.1)
* **UI Architecture**: Unity uGUI with CanvasScaler (Match Width or Height) & Screen Safe Area
* **Hosting**: Vercel Static Cloud Infrastructure (Brotli Content-Encoding, HTTPS, HTTP/2)

