# Lab 9 — Crystal Quest: Interactive 3D Game Using Unity

## Overview

Crystal Quest is an interactive 3D game developed using Unity 6 and C# as part of the Computer Graphics and Virtual Reality (CGVR) laboratory.

The player explores a 3D environment, collects five animated crystals, and completes the game by collecting all of them. The application features keyboard-based movement, jumping, third-person camera tracking, collision detection, a scoring system, a completion timer, and a victory screen.

The project demonstrates fundamental concepts of computer graphics, 3D transformations, physics, animation, and interactive game development.

## Objectives

- Develop an interactive 3D application using Unity.
- Implement player movement and jumping.
- Apply Rigidbody physics and collision detection.
- Implement third-person camera tracking.
- Create animated and collectible objects.
- Develop a real-time scoring and timer system.
- Design an interactive user interface.
- Build a standalone Windows application.

## Technologies Used

| Technology | Purpose |
|---|---|
| Unity 6 (6000.6.7f1) | Game development engine |
| C# | Gameplay programming |
| Universal Render Pipeline (URP) | 3D rendering |
| TextMeshPro | User interface text |
| Unity Physics | Collision detection and movement |
| Unity UI Canvas | Score, timer, and victory messages |

## Project Structure

```text
Lab9_CrystalQuest/
├── Assets/
│   ├── Scenes/
│   │   └── Lab9_CrystalQuest.unity
│   ├── Scripts/
│   │   ├── PlayerMovement.cs
│   │   ├── ThirdPersonCamera.cs
│   │   ├── CrystalCollectible.cs
│   │   └── GameManager.cs
│   ├── Materials/
│   ├── Prefabs/
│   ├── Models/
│   └── UI/
├── Packages/
├── ProjectSettings/
├── Builds/
└── README.md
```

Unity-generated `.meta` files are also included with the project assets.

## Features

### 1. Player Movement

The player is represented by a 3D capsule and can move freely across the environment using WASD or arrow keys.

Movement is implemented using C# and Rigidbody physics.

### 2. Jumping

The player can jump using the Space key. Gravity and collision detection allow the player to land naturally on the ground.

### 3. Third-Person Camera

A camera follows the player smoothly throughout the environment, providing a clear view of the character and surrounding objects.

### 4. Animated Crystals

Five collectible crystals are positioned across the game environment.

Each crystal continuously rotates and floats vertically using Unity transformations and mathematical animation.

### 5. Crystal Collection

Crystal collection is implemented using trigger colliders.

When the player touches a crystal:

- The crystal is detected through collision events.
- The crystal disappears from the scene.
- The collected crystal count increases.
- A collection message appears on the screen.

### 6. Scoring System

A real-time score counter displays the number of collected crystals.

**Example:** `Crystals: 3 / 5`

### 7. Completion Timer

A timer measures the time taken by the player to collect all five crystals.

The timer stops when the game is completed.

### 8. Victory Screen

After collecting all five crystals, the application displays a congratulations message containing the total crystals collected and the completion time.

The player can restart the game using the R key.

### 9. Environment Decoration

The game environment includes:

- A large ground plane.
- Four decorative trees.
- Custom materials and colours.
- Directional lighting.
- Animated collectible objects.

## Game Controls

| Key | Action |
|---|---|
| W / Up Arrow | Move forward |
| S / Down Arrow | Move backward |
| A / Left Arrow | Move left |
| D / Right Arrow | Move right |
| Space | Jump |
| R | Restart game |

## Implementation Details

### Player Movement

The `PlayerMovement.cs` script handles directional movement, jumping, and Rigidbody interactions.

### Camera Tracking

The `ThirdPersonCamera.cs` script updates the camera position relative to the player using smooth interpolation.

### Crystal Animation and Collection

The `CrystalCollectible.cs` script controls crystal rotation, floating animation, and trigger-based collection.

### Game Management

The `GameManager.cs` script manages:

- Crystal collection count.
- Score updates.
- Completion timer.
- Collection notifications.
- Victory conditions.
- Scene restart.

## How to Run the Unity Project

### Requirements

- Unity Hub
- Unity Editor 6000.6.7f1
- Universal Render Pipeline support

### Steps

1. Clone or download the repository.
2. Open Unity Hub.
3. Select **Add Project from Disk**.
4. Choose the project folder containing `Assets`, `Packages`, and `ProjectSettings`.
5. Open the project using Unity 6000.6.7f1.
6. Open `Assets/Scenes/Lab9_CrystalQuest.unity`.
7. Click **Play** to start the game.

## Windows Build

The project can be exported as a standalone Windows application.

### Build Instructions

1. Open **File → Build Profiles**.
2. Select Windows as the target platform.
3. Choose Intel 64-bit architecture.
4. Ensure the `Lab9_CrystalQuest` scene is included in the scene list.
5. Click **Build**.
6. Select the output directory.
7. Run the generated `.exe` file.

The executable must remain alongside its associated Unity data files.

## Computer Graphics Concepts Demonstrated

1. **3D Transformations:** Translation, rotation, and scaling of objects.
2. **Camera Transformations:** Third-person camera positioning and tracking.
3. **Animation:** Continuous rotation and vertical oscillation.
4. **Rendering:** Real-time 3D rendering using URP.
5. **Collision Detection:** Rigidbody physics and trigger colliders.
6. **Scene Management:** Restarting the application scene.
7. **User Interaction:** Keyboard controls and real-time UI updates.

## Expected Output

The application displays a playable 3D environment where the player can move, jump, and collect five animated crystals.

During gameplay, the score, elapsed time, and collection messages are displayed.

After collecting all five crystals, a congratulations screen displays the completion time and provides an option to restart the game.

## Conclusion

Crystal Quest demonstrates the development of an interactive 3D application using Unity and C#. The project integrates player controls, physics, camera tracking, object animation, collision detection, scoring, and user interface elements into a complete playable game.

The final application can be executed independently as a Windows build, demonstrating practical implementation of Computer Graphics and Virtual Reality laboratory concepts.