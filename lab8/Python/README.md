
# Lab 8 — Vertex and Fragment Shaders (Python)

## Overview

This project demonstrates the implementation of vertex and fragment shaders using Python, OpenGL, and GLSL.

The application renders a triangle with three different vertex colours. The GPU interpolates these colours across the triangle's surface, producing a smooth RGB gradient.

## Objectives

- Understand the graphics rendering pipeline.
- Implement vertex and fragment shaders using GLSL.
- Render a triangle using OpenGL.
- Apply RGB colours to individual vertices.
- Observe colour interpolation during rasterization.

## Technologies Used

- Python 3
- PyOpenGL
- OpenGL Shading Language (GLSL)
- GLUT / FreeGLUT
- NumPy (if required by the implementation)

## Project Structure

```text
Python/
├── main.py
├── requirements.txt
└── README.md
```

## Working Principle

### Vertex Shader

The vertex shader processes individual vertices and determines their positions in clip space.

It also passes vertex colour information to the next stage of the graphics pipeline.

Example:

```glsl
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aColor;

out vec3 vertexColor;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    vertexColor = aColor;
}
```

### Fragment Shader

The fragment shader determines the final colour of each fragment.

The colour values received from the vertex shader are interpolated automatically during rasterization.

```glsl
#version 330 core

in vec3 vertexColor;
out vec4 FragColor;

void main()
{
    FragColor = vec4(vertexColor, 1.0);
}
```

## Installation

### 1. Install Python

Install Python 3 and ensure it is available from the terminal.

### 2. Install Dependencies

From the Python project directory:

```bash
python -m pip install -r requirements.txt
```

Install FreeGLUT separately if required by your operating system and OpenGL setup.

### 3. Run the Application

```bash
python main.py
```

Replace `main.py` with the actual Python script filename if different.

## Expected Output

An OpenGL window displaying a triangle with:

- Red at the top vertex
- Green at the bottom-left vertex
- Blue at the bottom-right vertex
- Smoothly interpolated colours across the triangle

## Concepts Demonstrated

1. Vertex processing
2. Fragment processing
3. GPU shader programming
4. RGB colour interpolation
5. OpenGL rendering pipeline

## Conclusion

This experiment demonstrates how vertex and fragment shaders work together to render graphical objects. It provides a foundation for understanding GPU-based rendering and programmable graphics pipelines.
