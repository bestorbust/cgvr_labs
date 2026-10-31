
# Lab 8 — Vertex and Fragment Shaders (Unity)

## Overview

This project demonstrates vertex and fragment shader programming using Unity 6, ShaderLab, HLSL, and the Universal Render Pipeline (URP).

A custom triangle mesh is generated programmatically with three RGB vertex colours. A custom shader processes vertex positions and interpolates colours across the triangle.

The triangle rotates continuously, providing an interactive demonstration of shader-based rendering.

## Objectives

- Understand vertex and fragment shader operations.
- Create a custom 3D mesh programmatically.
- Implement a custom shader using HLSL.
- Demonstrate RGB colour interpolation.
- Apply transformations through C# scripting.
- Build an interactive Unity application.

## Technologies Used

- Unity 6.6 (6000.6.7f1)
- Universal Render Pipeline (URP)
- C#
- ShaderLab
- HLSL
- TextMeshPro
- Unity UI Canvas

## Project Structure

```text
Lab8_VertexFragmentShader/
├── Assets/
│   ├── Materials/
│   │   └── RGB_Triangle_Material.mat
│   ├── Scenes/
│   │   └── Lab8_ShaderDemo.unity
│   ├── Scripts/
│   │   ├── TriangleMesh.cs
│   │   └── TriangleRotation.cs
│   ├── Shaders/
│   │   └── RGBVertexShader.shader
│   └── Settings/
├── Packages/
├── ProjectSettings/
└── README.md
```

Unity also generates `.meta` files for assets and folders. These must be included in version control.

## Implementation

### 1. Triangle Mesh Generation

The `TriangleMesh.cs` script generates a triangle using three vertices.

Each vertex is assigned a different colour:

- Top vertex: Red
- Bottom-left vertex: Green
- Bottom-right vertex: Blue

The generated mesh is assigned to the object's MeshFilter.

### 2. Vertex Shader

The custom vertex shader transforms vertex positions from object space into clip space.

```hlsl
Varyings vert(Attributes input)
{
    Varyings output;

    output.positionCS =
        TransformObjectToHClip(input.positionOS.xyz);

    output.color = input.color;

    return output;
}
```

### 3. Fragment Shader

The fragment shader receives interpolated vertex colour values and returns the final fragment colour.

```hlsl
half4 frag(Varyings input) : SV_Target
{
    return half4(input.color.rgb, 1.0);
}
```

### 4. Triangle Rotation

The `TriangleRotation.cs` script continuously rotates the triangle around its Z-axis.

Rotation is frame-rate independent using `Time.deltaTime`.

### 5. User Interface

The scene includes a Unity Canvas with TextMeshPro elements displaying:

- Experiment title
- Vertex shader explanation
- Fragment shader explanation
- Keyboard controls

A dark background improves the visibility of the RGB triangle.

## Controls

| Key | Action |
|-----|--------|
| Space | Pause / Resume rotation |
| R | Reset triangle rotation |

## How to Run

### Requirements

- Unity Hub
- Unity Editor 6000.6.7f1
- Universal Render Pipeline support

### Steps

1. Clone or download the repository.
2. Open Unity Hub.
3. Select **Add Project from Disk**.
4. Choose the Unity project directory containing `Assets`, `Packages`, and `ProjectSettings`.
5. Open the project using Unity 6000.6.7f1.
6. Open `Assets/Scenes/Lab8_ShaderDemo.unity`.
7. Press **Play**.

## Building the Application

1. Open **File → Build Profiles**.
2. Select the Windows platform.
3. Include `Lab8_ShaderDemo` in the scene list.
4. Select **Build**.
5. Choose an output folder outside the `Assets` directory.
6. Run the generated Windows executable.

## Expected Output

The application displays:

- A rotating triangular mesh
- Smooth RGB colour interpolation
- A dark background
- Vertex and fragment shader explanations
- Keyboard controls for rotation

## Concepts Demonstrated

1. Custom mesh generation
2. Vertex transformations
3. Fragment shading
4. Colour interpolation
5. ShaderLab and HLSL
6. Object transformations and animation
7. Interactive rendering in Unity

## Conclusion

This experiment demonstrates the implementation of programmable vertex and fragment shaders in Unity. It combines GPU-based rendering, procedural mesh generation, and C# scripting to create an interactive visualization of the graphics pipeline.


## Python Implementation

This repository also includes a Python/OpenGL implementation of
vertex and fragment shaders.

The Python implementation demonstrates RGB colour interpolation
using OpenGL and GLSL, while the Unity implementation uses
ShaderLab, HLSL, and the Universal Render Pipeline.

For implementation details and execution instructions, see
[Python Implementation](Python/README.md).
