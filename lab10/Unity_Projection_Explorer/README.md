# Lab 10 — Projection Explorer (Unity 6000.6.7f1)

Compare **Orthographic (Parallel)** and **Perspective** 3D viewing projections using two synchronized side-by-side viewports observing the same rotating 6-colored cube.

---

## Quick Start

1. Open **Unity Hub**.
2. Click **Add** → **Add project from disk**.
3. Select this directory: **`Unity_Projection_Explorer`** (not the parent `lab10`).
4. Open with **Unity 6000.6.7f1** (Built-in Render Pipeline).
5. Open the scene located at:
   ```text
   Assets/Scenes/Lab10_3DViewingPipeline.unity
   ```
   *(Or click top menu: **`CGVR Labs → Lab 10: Projection Explorer → Open / Setup Scene`**)*
6. Press **Play** and interact in the Game window.

---

## Interactive Controls

| Input | Action |
|---|---|
| <kbd>Space</kbd> | Pause / Resume rotation animation |
| <kbd>R</kbd> | Reset cube rotation orientation |
| <kbd>↑</kbd> / <kbd>↓</kbd> | Increase / Decrease rotation speed |
| <kbd>H</kbd> | Toggle HUD on-screen overlay |
| UI Sliders | Adjust Orthographic Size and Perspective FOV |

---

## Build Standalone Executable

To export a standalone Windows binary:
- In Unity Editor, choose **`CGVR Labs → Lab 10: Projection Explorer → Build Standalone Windows Player`**.
- The executable will be saved to `Builds/Lab10_ProjectionExplorer.exe`.

---

## Core Assets & Code

- **`Assets/Scripts/Lab10ViewingPipelineDemo.cs`**: Dual camera setup, procedural colored cube generation, wireframe outlines, rotation physics, and HUD.
- **`Assets/Shaders/Lab10FaceColors.shader`**: Unlit vertex-colored shader with `Unlit/Color` fallback.
- **`Assets/Editor/Lab10SceneMenu.cs`**: Unity Editor menu tools for scene initialization, validation, and standalone build automation.
- **`Assets/Scenes/Lab10_3DViewingPipeline.unity`**: Pre-configured scene with the projection explorer component.

---

## Demonstration Video

A complete recorded video demonstration of this Unity project is available at:
`../Lab 10 - Projection Explorer 2026-10-29 23-49-10.mp4`
