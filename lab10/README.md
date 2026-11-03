# Lab 10 — 3D Viewing Pipeline and Projection Explorer

## Overview

**Lab 10 — Projection Explorer** is an interactive computer graphics laboratory project developed for the Computer Graphics and Virtual Reality (CGVR, Semester 7) course.

The project provides an interactive, side-by-side comparative analysis of the fundamental **3D Viewing Pipeline**, contrasting **Orthographic (Parallel) Projection** on the left viewport against **Perspective Projection** on the right viewport. Both cameras simultaneously track the same rotating 3D colored cube, allowing immediate visual and mathematical appreciation of foreshortening, field-of-view, projection rays, viewing frustums, and coordinate transformations.

The repository includes a complete **Unity 6 project**, an independent **PyOpenGL + GLUT alternative**, full source code, documentation, and a recorded **demonstration output video**.

---

## Objectives

- Understand the stages of the synthetic camera model and the 3D viewing pipeline:
  $$\text{Object Coordinates} \longrightarrow \text{World} \longrightarrow \text{View (Eye)} \longrightarrow \text{Clip} \longrightarrow \text{NDC} \longrightarrow \text{Screen (Viewport)}$$
- Implement a dual-camera viewport split in real time using viewport rectangles:
  - Left camera: Orthographic projection ($[0.0, 0.0, 0.5, 1.0]$)
  - Right camera: Perspective projection ($[0.5, 0.0, 0.5, 1.0]$)
- Generate a procedural 6-colored unit cube with unlit vertex-colored shading and wireframe edge highlights.
- Implement real-time interactive controls to dynamically adjust:
  - Rotation speeds and axes
  - Orthographic viewing volume size ($2.2$ to $5.0$)
  - Perspective Field of View (FOV: $30^\circ$ to $70^\circ$)
  - HUD overlay display toggling
- Provide both a native Unity 6 implementation and a lightweight Python (PyOpenGL/GLUT) alternative.
- Package and automate standalone Windows Player compilation.

---

## Output Video

The complete execution and interactive manipulation of the projection explorer is recorded and archived directly in this directory:

- **Video File:** [`Lab 10 - Projection Explorer 2026-10-29 23-49-10.mp4`](./Lab%2010%20-%20Projection%20Explorer%202026-10-29%2023-49-10.mp4)
- **Duration / Size:** High-definition screen recording (~33.9 MB).
- **Contents Demonstrated:** Side-by-side viewport rendering, live orthographic vs. perspective projection comparison, rotation pause/resume, speed modulation, orthographic scale tuning, perspective FOV distortion/expansion, and HUD telemetry.

---

## Technologies Used

| Technology | Purpose |
|---|---|
| **Unity 6 (6000.6.7f1)** | Primary graphics engine and viewport renderer |
| **C# (.NET / Unity API)** | Procedural mesh generation, camera controls, OnGUI HUD, and editor automation |
| **Built-in Render Pipeline / ShaderLab** | High-performance vertex-colored unlit shader (`Lab10FaceColors.shader`) |
| **Python 3.10+ / PyOpenGL / GLUT** | Cross-platform desktop standalone OpenGL viewing pipeline alternative |
| **Git / GitHub** | Version control with clean asset tracking and ignore policies |

---

## Project Structure

```text
lab10/
├── Lab 10 - Projection Explorer 2026-10-29 23-49-10.mp4   # Output demonstration video
├── README.md                                             # Comprehensive laboratory report
├── .gitignore                                            # Local Git ignore rules
├── projection_explorer.py                                # Independent PyOpenGL + GLUT alternative
└── Unity_Projection_Explorer/                            # Unity 6 project root
    ├── .gitignore                                        # Unity cache/temp ignore rules
    ├── README.md                                         # Unity-specific quick start guide
    ├── Packages/
    │   ├── manifest.json                                 # Unity package dependencies
    │   └── packages-lock.json
    ├── ProjectSettings/                                  # Unity input, tag, and quality settings
    │   ├── EditorBuildSettings.asset
    │   ├── GraphicsSettings.asset
    │   ├── QualitySettings.asset
    │   └── ProjectSettings.asset
    └── Assets/
        ├── Scenes/
        │   ├── Lab10_3DViewingPipeline.unity             # Primary dual-camera interactive scene
        │   └── Lab10_3DViewingPipeline.unity.meta
        ├── Scripts/
        │   ├── Lab10ViewingPipelineDemo.cs               # Procedural mesh, camera rig & HUD logic
        │   └── Lab10ViewingPipelineDemo.cs.meta
        ├── Shaders/
        │   ├── Lab10FaceColors.shader                    # Vertex-colored unlit shader with fallback
        │   └── Lab10FaceColors.shader.meta
        └── Editor/
            ├── Lab10SceneMenu.cs                         # Editor menu: Setup scene & Build Windows
            └── Lab10SceneMenu.cs.meta
```

---

## Computer Graphics Theory: The 3D Viewing Pipeline

The 3D viewing pipeline describes the series of coordinate system transformations that transform 3D geometric primitives defined in local modeling space into 2D rasterized pixel coordinates on the display:

```
+--------------------------+
| Model / Object Space     | Local coordinates (x_m, y_m, z_m, 1)
+--------------------------+
             |
             | Model Matrix (M) — Translation, Rotation, Scale
             v
+--------------------------+
| World Space              | Global scene positioning (x_w, y_w, z_w, 1)
+--------------------------+
             |
             | View Matrix (V) — Camera position, orientation (LookAt)
             v
+--------------------------+
| View / Eye Space         | Relative to camera frame (x_e, y_e, z_e, 1)
+--------------------------+
             |
             | Projection Matrix (P) — Orthographic or Perspective
             v
+--------------------------+
| Clip Space               | Homogeneous clip coordinates (x_c, y_c, z_c, w_c)
| (Clipping against frustum)| -w_c <= x_c, y_c, z_c <= w_c
+--------------------------+
             |
             | Perspective Division (Divide by w_c)
             v
+--------------------------+
| Normalized Device Coord. | NDC unit cube [-1, 1]^3
+--------------------------+
             |
             | Viewport Transformation (Scale to pixel rect)
             v
+--------------------------+
| Screen / Viewport Space  | Rasterized framebuffer pixels [X_min, Y_min, W, H]
+--------------------------+
```

### 1. Model to World Transformation
Transforms vertices from their local authoring space to common world space using rotation matrix $R$, scale matrix $S$, and translation matrix $T$:
$$M_{\text{model}} = T \cdot R \cdot S$$

### 2. World to View (Eye) Transformation
Aligns the camera's eye at the origin with the gaze direction pointing along the $-Z$ axis:
$$V = \begin{bmatrix} R_{\text{cam}}^T & -R_{\text{cam}}^T \cdot \mathbf{p}_{\text{cam}} \\ \mathbf{0}^T & 1 \end{bmatrix}$$

### 3. Projection Transformations: Orthographic vs. Perspective

#### Orthographic (Parallel) Projection
- Projects points along parallel projection rays orthogonal to the projection plane.
- The viewing volume is a rectangular box (cuboid) bounded by $[l, r]$, $[b, t]$, and $[n, f]$.
- **Orthographic Projection Matrix:**
  $$P_{\text{ortho}} = \begin{bmatrix} \frac{2}{r - l} & 0 & 0 & -\frac{r + l}{r - l} \\ 0 & \frac{2}{t - b} & 0 & -\frac{t + b}{t - b} \\ 0 & 0 & -\frac{2}{f - n} & -\frac{f + n}{f - n} \\ 0 & 0 & 0 & 1 \end{bmatrix}$$
- **Characteristics:**
  - Parallel lines in 3D remain strictly parallel in the 2D projection.
  - Object projection size is completely invariant to distance from the camera ($z_{\text{eye}}$).
  - Preserves exact engineering proportions, distances, and scales (used in CAD, architectural blueprints, and technical diagrams).

#### Perspective Projection
- Projects points along rays converging at a single center of projection (the camera eye/focal point).
- The viewing volume is a truncated pyramid (**frustum**) defined by field of view ($\theta_{\text{FOV}}$), aspect ratio ($a$), and near/far clipping planes ($n, f$).
- **Perspective Projection Matrix:**
  $$P_{\text{persp}} = \begin{bmatrix} \frac{1}{a \cdot \tan(\theta/2)} & 0 & 0 & 0 \\ 0 & \frac{1}{\tan(\theta/2)} & 0 & 0 \\ 0 & 0 & -\frac{f + n}{f - n} & -\frac{2fn}{f - n} \\ 0 & 0 & -1 & 0 \end{bmatrix}$$
- **Characteristics:**
  - Creates realistic **foreshortening**: distant objects appear smaller ($x_{\text{screen}} \propto \frac{1}{z}$).
  - Parallel lines receding in depth converge at a vanishing point.
  - Mimics human ocular vision and optical camera lenses.

---

## Detailed Projection Comparison Table

| Attribute | Orthographic Projection (Left Viewport) | Perspective Projection (Right Viewport) |
|---|---|---|
| **Projection Center** | At infinity ($\infty$) | At the camera focal point |
| **Projection Rays** | Mutually parallel and perpendicular to image plane | Converging at the camera center |
| **Viewing Volume** | Rectangular parallelepiped (cuboid) | Truncated pyramid (frustum) |
| **Foreshortening** | **None** (identical size at any depth) | **Yes** (objects shrink with distance $\propto 1/z$) |
| **Parallel Lines** | Remain parallel across all angles | Receding parallel lines converge at vanishing points |
| **Scale / Proportions** | True metric proportions preserved | Distorted by depth and perspective angle |
| **Field of View / Zoom** | Controlled via half-height Orthographic Size | Controlled via angular Field of View ($\theta_{\text{FOV}}$ in degrees) |
| **Primary Applications** | CAD, architectural schematics, isometric 2D/3D games | Realistic 3D games, VR/AR, film CGI, simulation |

---

## Dual-Camera Viewport Architecture in Unity

Rather than switching scenes or toggling cameras on and off, the application uses **Unity camera viewport rects** (`Camera.rect`) to partition the screen into two independent, simultaneous rendering viewports:

```
+-----------------------------------+-----------------------------------+
|  Left Viewport: ORTHOGRAPHIC     |  Right Viewport: PERSPECTIVE      |
|  rect = (0.0f, 0.0f, 0.5f, 1.0f)   |  rect = (0.5f, 0.0f, 0.5f, 1.0f)   |
|                                   |                                   |
|  • cam.orthographic = true        |  • cam.orthographic = false       |
|  • cam.orthographicSize = 3.2f    |  • cam.fieldOfView = 50.0f        |
|  • ClearFlags = SolidColor        |  • ClearFlags = DepthOnly         |
|  • Constant scale, parallel rays  |  • Natural depth foreshortening   |
+-----------------------------------+-----------------------------------+
```

### Shared 3D Target Model
- A single procedural 3D unit cube ($2 \times 2 \times 2$ units) centered at $(0, 0, 0)$.
- **6 Distinct Face Colors:**
  - Front ($+Z$): Cyan `(0.15, 0.78, 0.90)`
  - Back ($-Z$): Purple `(0.75, 0.35, 0.90)`
  - Right ($+X$): Coral / Orange `(0.98, 0.48, 0.33)`
  - Left ($-X$): Green `(0.28, 0.76, 0.47)`
  - Top ($+Y$): Amber / Yellow `(0.96, 0.76, 0.22)`
  - Bottom ($-Y$): Magenta / Pink `(0.85, 0.25, 0.48)`
- **Contrast Wireframe Edges:** 12 boundary lines highlighted in crisp dark charcoal `(0.1, 0.1, 0.15)` via `LineRenderer` to clearly accentuate face geometry and vanishing line convergence under perspective.

---

## Interactive Controls

| Control | Input Key / UI | Action |
|---|---|---|
| **Toggle Animation** | <kbd>Space</kbd> | Pause or resume continuous cube rotation |
| **Reset Orientation** | <kbd>R</kbd> | Reset cube rotation angle to default state ($25^\circ$) |
| **Speed Adjustment** | <kbd>↑</kbd> / <kbd>↓</kbd> Arrows | Increase or decrease rotation speed ($\pm 15^\circ/\text{sec}$) |
| **Toggle HUD Overlay** | <kbd>H</kbd> | Show or hide the telemetry and keyboard guide panel |
| **Ortho Size Slider** | UI Slider ($2.2$ – $5.0$) | Scale the orthographic camera's view volume height |
| **Perspective FOV Slider** | UI Slider ($30^\circ$ – $70^\circ$) | Expand or tighten the perspective camera's field of view |

---

## Source Files & Implementation Breakdown

### 1. `Lab10ViewingPipelineDemo.cs`
- **Location:** `Unity_Projection_Explorer/Assets/Scripts/Lab10ViewingPipelineDemo.cs`
- **Responsibilities:**
  - Instantiates and configures the dual camera rig (`Camera_Ortho` and `Camera_Perspective`) with split-screen viewports (`0.0`–`0.5` and `0.5`–`1.0`).
  - Procedurally builds the 24-vertex cube mesh, assigning unique vertex colors per face.
  - Generates contrasting wireframe outlines using `LineRenderer` loops.
  - Updates cube rotation every frame using delta-time scaled angular velocity.
  - Draws an informative GUI HUD displaying live rotation speed, current FOV, orthographic size, and hotkey instructions.

### 2. `Lab10FaceColors.shader`
- **Location:** `Unity_Projection_Explorer/Assets/Shaders/Lab10FaceColors.shader`
- **Responsibilities:**
  - Custom unlit ShaderLab shader that accepts per-vertex colors directly from the mesh:
    $$C_{\text{frag}} = C_{\text{vertex}}$$
  - Bypasses lighting computations for pristine chromatic comparisons.
  - Includes a safe `Fallback "Unlit/Color"` to guarantee rock-solid cross-platform rendering across all Unity render pipelines.

### 3. `Lab10SceneMenu.cs`
- **Location:** `Unity_Projection_Explorer/Assets/Editor/Lab10SceneMenu.cs`
- **Responsibilities:**
  - Adds a dedicated top-level **`CGVR Labs → Lab 10: Projection Explorer`** menu in Unity Editor.
  - **Open / Setup Scene:** Creates or opens `Lab10_3DViewingPipeline.unity` and instantiates the demonstration controller.
  - **Validate Scene Setup:** Verifies camera configurations, viewports, materials, and shaders.
  - **Build Standalone Windows Player:** One-click automated export using `BuildPipeline.BuildPlayer` into `Builds/Lab10_ProjectionExplorer.exe`.

### 4. `projection_explorer.py` (Python Alternative)
- **Location:** `lab10/projection_explorer.py`
- **Responsibilities:**
  - Standalone desktop application written in Python using PyOpenGL and FreeGLUT.
  - Computes exact `glOrtho` and `gluPerspective` viewing projections.
  - Renders the identical 6-colored cube with wireframe outlines side-by-side using `glViewport(0, 0, W/2, H)` and `glViewport(W/2, 0, W/2, H)`.
  - Supports keyboard shortcuts (`Space`, `R`, `+`, `-`, `W`, `Q`, `Esc`).

---

## How to Run

### Option A: Unity Editor (Recommended)

1. **Open Unity Hub.**
2. Click **Add** → **Add project from disk**.
3. Select the folder:
   ```text
   cgvr_labs/lab10/Unity_Projection_Explorer
   ```
4. Open with **Unity 6000.6.7f1** (Built-in Render Pipeline).
5. In the Project window, navigate to:
   ```text
   Assets/Scenes/Lab10_3DViewingPipeline.unity
   ```
   *(Alternatively, click top menu: `CGVR Labs` → `Lab 10: Projection Explorer` → `Open / Setup Scene`).*
6. Press the **Play** button at the top of the Unity Editor.
7. Switch to the **Game** tab to interact with both viewports.

### Option B: Standalone Windows Build

1. Inside Unity Editor, open the top menu:
   ```text
   CGVR Labs → Lab 10: Projection Explorer → Build Standalone Windows Player
   ```
2. Unity will automatically build the executable to `Unity_Projection_Explorer/Builds/Lab10_ProjectionExplorer.exe`.
3. Launch the generated `.exe` file to run the simulation natively without launching Unity Editor.

### Option C: Python + PyOpenGL Alternative

For a lightweight standalone run outside of Unity:

1. Open a terminal and ensure Python 3.10+ is installed.
2. Install PyOpenGL dependencies:
   ```powershell
   python -m pip install PyOpenGL PyOpenGL_accelerate
   ```
3. Run the script:
   ```powershell
   cd cgvr_labs/lab10
   python projection_explorer.py
   ```
4. A window will launch displaying the identical dual-viewport comparison.

---

## Computer Graphics Concepts Demonstrated

1. **Synthetic Camera Model:** Definition of camera position ($\mathbf{e}$), look-at target ($\mathbf{g}$), and up vector ($\mathbf{u}$).
2. **Coordinate Spaces & Transformations:** Progression from Local Object Space to Screen Pixels via matrix multiplications ($M \cdot V \cdot P$).
3. **Parallel Projection Geometry:** Preservation of affine line parallelism and dimensional constancy regardless of depth.
4. **Perspective Frustum & Foreshortening:** Center of projection convergence, non-linear depth compression ($z_{\text{clip}}/w$), and perspective distortion.
5. **Multi-Camera Viewport Segmentation:** Normalization of screen space into discrete sub-viewports using normalized coordinates ($[0, 1] \times [0, 1]$).
6. **Vertex Color Interpolation & Unlit Shading:** Attribute passing from vertex buffers to fragment shaders without directional lighting attenuation.
7. **Interactive GUI & Camera Dynamics:** Real-time user parameter modification demonstrating optical lens adjustments (telephoto vs wide-angle perspective).

---

## Conclusion

Lab 10 successfully demonstrates the complete 3D viewing pipeline and provides an immediate, intuitive comparison between orthographic and perspective projections. By coupling two synchronized viewports to a single animated 3D target, the fundamental mathematical differences—parallel projection rays versus converging focal rays, metric invariance versus realistic foreshortening—are clearly illustrated and interactively verifiable.

The accompanying demonstration video, comprehensive Unity project, editor build automation, and Python alternative form a complete, self-contained educational package for Computer Graphics and Virtual Reality.
