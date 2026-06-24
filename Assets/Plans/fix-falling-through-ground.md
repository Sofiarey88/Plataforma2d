# Project Overview 
- Game Title: Plataforma2d (2D Platformer)
- High-Level Concept: A 2D platformer game featuring a frog character ("Rana") and dynamic elements like moving platforms, hazards, and enemy characters.
- Players: Single player
- Inspiration / Reference Games: Classic 2D platformers (Mario, Celeste)
- Tone / Art Direction: Retro 2D Pixel Art
- Target Platform: Standalone Windows PC
- Screen Orientation / Resolution: Landscape
- Render Pipeline: Universal RP (URP)

# Game Mechanics 
## Core Gameplay Loop
The player controls a frog character ("Rana") navigating levels, jumping across static and moving platforms, stomping enemies, avoiding hazards, and reaching the level exit.
## Controls and Input Methods
Keyboard/Controller movement (A/D or Arrow keys to move, Space to jump).

# UI
(No modifications to UI elements are required for this physics/collision fix.)

# Key Asset & Context
- **Assets/Prefab/Rana.prefab**: Player character prefab containing Rigidbody2D and BoxCollider2D.
- **Assets/Script/Player.cs**: Character movement, grounding, and collision detection script.
- **Assets/Prefab/PlataformaMovilV Variant.prefab** & **Assets/Prefab/PlataformaMovilH.prefab**: Moving platform prefabs containing the `EscenarioMovil` script but missing `Rigidbody2D` components.
- **Assets/Script/EscenarioMovil.cs**: Base class for moving platforms, handles Rigidbody2D-based movement in FixedUpdate if a Rigidbody2D is present.

# Implementation Steps

## Step 1: Implement Robust Grounding Logic in `Player.cs`
- **Description**: Update collision enter/exit logic in `Player.cs` to use a `HashSet<Collider2D>` to count and track active ground colliders. This prevents the player from losing grounded status when walking over seams between adjacent ground tile colliders.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 2: Set Player Rigidbody2D Collision Detection to Continuous
- **Description**: Update the Player's Rigidbody2D `Collision Detection Mode` to `Continuous` in `Assets/Prefab/Rana.prefab` (and the active scene instance) to prevent tunneling through thin colliders at high downward velocities.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

## Step 3: Add Rigidbody2D to Moving Platforms
- **Description**: Add a `Rigidbody2D` component to `PlataformaMovilV Variant.prefab` and `PlataformaMovilH.prefab`, configured as `Kinematic` with `Freeze Rotation Z` enabled. Ensure `useRigidbody2D` is checked on their script components. This switches their movement from teleporting in `Update` to smooth physical translation in `FixedUpdate` via `MovePosition`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

# Verification & Testing
- **Grounding Test**: Walk across adjacent ground tile seams. Verify that `isGrounded` remains true and jumping is always responsive.
- **High-Velocity Fall Test**: Jump from high altitudes and activate Bullet Time (which multiplies gravity) to reach maximum falling speeds. Verify the player never tunnels through thin (0.15-0.20 thickness) floor colliders.
- **Moving Platform Ride Test**: Ride vertical and horizontal moving platforms. Verify the character moves smoothly with them and does not fall through during direction shifts.
