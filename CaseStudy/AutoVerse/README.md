# AutoVerse — Premium 3D Interactive Car Customization Showroom

[![Live WebGL Demo](https://img.shields.io/badge/Live_Demo-auto--verse--plum.vercel.app-00E5FF?style=for-the-badge&logo=vercel)](https://auto-verse-plum.vercel.app/)
[![Unity 6 Version](https://img.shields.io/badge/Unity_6-6000.6.7f1-000000?style=for-the-badge&logo=unity)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Pipeline-Universal_Render_Pipeline_(URP)-4B5563?style=for-the-badge)](https://unity.com/srp/universal-render-pipeline)
[![Target Platform](https://img.shields.io/badge/Platform-WebGL_2.0_|_Windows_Standalone-059669?style=for-the-badge)](https://auto-verse-plum.vercel.app/)

> **Live Production URL**: [https://auto-verse-plum.vercel.app/](https://auto-verse-plum.vercel.app/)

**AutoVerse** is a production-ready, interactive 3D vehicle customization showroom built in **Unity 6.6 (`6000.6.7f1`)** using the **Universal Render Pipeline (URP)** and C#.

The application runs smoothly across **Desktop (Windows Standalone)**, **Web (WebGL 2.0)**, and **Mobile Devices (iOS / Android browsers)** with an adaptive, responsive UI, rich architectural showroom, and complete touch/mouse interaction isolation.

---

## 1. Key Features

- **8 Distinct Vehicles**:
  1. `car_sports` — **SPORTS COUPE** (Exotic mid-engine performance coupe)
  2. `car_sedan` — **EXECUTIVE SEDAN** (Executive luxury 4-door sedan)
  3. `car_hatchback` — **COMPACT HATCH** (Urban daily driver)
  4. `car_suv` — **URBAN SUV** (Full-size luxury sport utility vehicle)
  5. `race_car` — **GT FORMULA RACER** (Open-cockpit high-downforce formula car)
  6. `car_police` — **POLICE INTERCEPTOR** (Law enforcement pursuit vehicle with emergency light bar)
  7. `car_taxi` — **CITY TAXI** (Metropolitan cab with illuminated roof sign)
  8. `golf_cart` — **LUXURY GOLF CART** (Open resort electric cruiser)

- **Deep Paint & Surface Customization**:
  - **18 Selectable Colors**: Pearl White, Midnight Black, Graphite, Arctic Silver, Racing Red, Crimson, Sunset Orange, Solar Yellow, Royal Blue, Sky Blue, Emerald Green, Forest Green, Violet, Rose Gold, Champagne, Bronze, Teal, and Candy Pink.
  - **4 Paint Finishes**: Gloss (high smoothness, clearcoat reflection), Metallic (high metallic, balanced micro-roughness), Matte (zero metallic, diffuse light absorption), and Satin (subtle velvety sheen).
  - **Non-destructive Runtime Instances**: Cloned per-vehicle runtime materials preserve original assets; instant reset restores factory state without leaks.

- **Component & Accessory Customization**:
  - **5 Wheel Finishes**: Stealth Black, Polished Silver, Gunmetal, Liquid Gold, and Burnt Bronze.
  - **5 Brake Caliper Colors**: Brembo Racing Red, Acid Yellow, Electric Blue, Acid Lime Green, and Stealth Gloss Black.
  - **6 Window Tints**: Clear Glass ($35\%$), Light Smoke ($55\%$), Dark Charcoal ($82\%$), Limo Blackout ($96\%$), Sunset Gold Chameleon, and Cyber Blue Mirror.
  - **5 Headlight Emission Styles**: Xenon White ($6500\text{K}$), Warm Halogen ($3200\text{K}$), Ice Blue LED ($8500\text{K}$), Cyber Violet LED, and Amber Gold DRL.
  - **6 Underglow Neon Colors**: Electric Cyan, Neon Magenta, Acid Green, Amber Gold, Ice White, and Royal Violet with dynamic downward ground-illuminating point lights.
  - **4 Procedural Accessories**: Roof Light Bar, Rear Wing / Spoiler, Dual Racing Stripes, and Underglow Neon Lighting (dynamically sized and anchored to vehicle bounds).

- **Architectural Showroom & Dynamic Lighting**:
  - Full 360-degree luxury architectural showroom (seamless polished obsidian floor $45\,\text{m} \times 45\,\text{m}$, surrounding panel walls, and suspended overhead studio softbox luminaire).
  - Cleaned up all legacy tripod meshes, floor spotlight fixtures, and radial ground spoke lights for a clean, professional studio environment.
  - Backlit `"AUTOVERSE"` luxury presentation wall, elevated carbon feature stage, flanking vertical LED light columns, showcase window bay, and low-profile turntable with flush ambient LED halo ring.
  - **4 Dynamic Lighting Presets**: Luxury Studio (warm champagne & tungsten), Midnight Studio (high-tech ice blue & cyan rim), Bright Gallery (pure daylight white high-key pavilion), and Neon Garage (cyberpunk magenta & electric cyan) with real-time floor ambiance and architectural lighting sync.
  - Smooth turntable auto-rotation (15°/sec without camera jumps).

- **Responsive & Touch-Friendly UI**:
  - Native Unity uGUI Canvas dynamically adapting between **Desktop**, **Tablet**, **Mobile Portrait**, and **Mobile Landscape**.
  - **Top Floating Bar**: Clean brand logo, centered `< VEHICLE >` switcher, and quick action pills (`Reset`, `Spin: ON/OFF`, `Studio`, `Keys`). Guaranteed fixed anchoring prevents top-bar clipping across any screen ratio or browser iframe.
  - **Bottom Customization Hub**: Modern floating shelf with 8 quick-navigation category tabs (`Paint`, `Wheels`, `Glass`, `Lights`, `Neon`, `Accessories`, `Camera`, `Studio`).
  - **Circular Color Swatches**: Antialiased circular color swatches with active selection rings and live selection label.
  - **Keyboard Controls Modal**: Clean toggleable modal popover (`Keys` button or `?`/`Tab`) replacing permanent floating boxes.
  - **Interaction Isolation & Deadzone**: Drag-threshold deadzone (10px) and multi-layer GraphicRaycaster + screen margin guards ensure clicking options or moving the cursor never causes camera rotations.
  - **Touch Controls**: 1-finger deliberate orbit, 2-finger pinch zoom, 2-finger pan, with complete touch isolation preventing accidental camera movement when interacting with buttons.
  - **6 Camera View Presets**: Hero View (3/4 angle), Front (facing headlights), Side (profile), Rear (facing spoiler), Top (overhead), and Wheel close-up.

---

## 2. Vehicle Compatibility Matrix

Each vehicle has its own distinct mesh topology and material slot layout:

| Vehicle | Body Paint Slot | Glass Tint Slot | Wheel Rim Slot | Headlight Slot | Livery / Unique Parts | Unsupported Features |
| :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| **`car_sports`** | 0 (`orange`) | 1 (`glass`) | 3 (`hub`) | 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) | None — Full Support |
| **`car_sedan`** | 0 (`carblue`) | 1 (`glass`) | 3 (`hub`) | 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) | None — Full Support |
| **`car_hatchback`** | 0 (`carred`) | 1 (`glass`) | 3 (`hub`) | 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) | None — Full Support |
| **`car_suv`** | 0 (`cargreen`) | 1 (`glass`) | 3 (`hub`) | 4 (`headlight`) | Slot 2 (`bumper`), Slot 5 (`interior`) | None — Full Support |
| **`race_car`** | 0 (`carred`) | *N/A* | 5 (`hub`) | *N/A* | Slots 1 & 2 (`black`/`yellow`), Slot 3 (`dark`) | No Glass / Headlights (Open Formula Cockpit) |
| **`car_police`** | 0 (`white`) | 1 (`glass`) | 4 (`hub`) | 5 (`headlight`) | Slot 2 (`navy`), Slot 8 (`siren`) | None — Full Support |
| **`car_taxi`** | 0 (`taxi`) | 1 (`glass`) | 3 (`hub`) | 4 (`headlight`) | Slot 2 (`interior`), Slot 7 (`white` sign) | None — Full Support |
| **`golf_cart`** | 3 (`lime`) | 4 (`glass`) | 8 (`hub`) | 9 (`headlight`) | Slot 0 (`canopy`), Slot 1 (`seat`), Slot 2 (`frame`) | None — Full Support |

---

## 3. Keyboard & Touch Controls

| Function | Keyboard Shortcut | Mouse Control | Touch Gesture |
| :--- | :---: | :---: | :---: |
| **Orbit Camera** | Left / Right Arrow | Left-Click & Drag (10px deadzone) | 1-Finger Drag |
| **Pan Camera** | — | Right-Click & Drag | 2-Finger Drag |
| **Zoom View** | Up / Down Arrow | Mouse Scroll Wheel | 2-Finger Pinch In / Out |
| **Select Vehicle** | Keys `0` through `7` | `<` and `>` Arrow Buttons | Tap Arrow or Key |
| **Cycle Colors** | `C` | Circular Swatches | Tap Color Swatch |
| **Cycle Finishes** | `G` / `M` / `N` / `S` | Finish Tab Buttons | Tap Finish Button |
| **Cycle Lighting** | `1` – `4` / `F1` – `F4` | Top Bar "Studio" Pill | Tap Studio Pill |
| **Turntable Toggle** | `A` | Top Bar "Spin: ON/OFF" | Tap Spin Button |
| **Reset Customization** | `X` / `Backspace` | "Reset" Button | Tap Reset Button |
| **Camera Presets** | — | Bottom Preset Buttons | Tap Hero / Front / Side / etc. |

---

## 4. Architecture & Script Organization

```
Assets/
├── Scenes/
│   └── AutoVerse_Showwroom.unity       # Main interactive showroom scene
├── Scripts/
│   ├── Core/
│   │   └── CarSelector.cs              # Vehicle selection, activation & event dispatch
│   ├── Customization/
│   │   ├── VehiclePainter.cs           # Paint, finish, wheels, calipers, tints, headlights & reset
│   │   └── VehicleAccessoryManager.cs  # Procedural bounds-fitted spoilers, stripes, lights, neon
│   ├── Camera/
│   │   └── ShowroomCamera.cs           # Orbit, pinch zoom, UI click isolation, presets
│   ├── Showroom/
│   │   ├── ShowroomLightingManager.cs  # Studio key/fill/softbox rig & 4 lighting styles
│   │   └── ShowroomEnvironmentManager.cs # 360° architectural walls, floor, halo turntable
│   ├── UI/
│   │   └── AutoVerseShowroomUI.cs      # Responsive top floating bar & customization dock
│   └── Editor/
│       └── AutoVerseBuildPipeline.cs   # Automated validation tests & build pipelines
```

---

## 5. WebGL Deployment & Hosting

The WebGL build is deployed directly to **Vercel** with Brotli compression and strict caching headers via `vercel.json`:
- `.data.br`: `Content-Encoding: br`, `Content-Type: application/octet-stream`
- `.wasm.br`: `Content-Encoding: br`, `Content-Type: application/wasm`
- `.js.br`: `Content-Encoding: br`, `Content-Type: application/javascript`

Live URL: **[https://auto-verse-plum.vercel.app/](https://auto-verse-plum.vercel.app/)**
