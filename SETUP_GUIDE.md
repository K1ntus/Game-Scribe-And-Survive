# Unity Scene Setup Guide - Gym_Rhythm

This guide will walk you through setting up the Gym_Rhythm scene in Unity Editor.

## Prerequisites
- Unity 2022.3+ (LTS) installed
- Project opened in Unity
- All scripts in Assets/Scripts/ loaded

## Step-by-Step Setup

### 1. Create the Scene
1. In Unity, go to **File > New Scene**
2. Save as `Gym_Rhythm.unity` in `Assets/Scenes/`

### 2. Create the Conductor (Global Rhythm Clock)

1. **Create GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Name it: `Conductor`
   - Position: (0, 0, 0)

2. **Add Conductor Component:**
   - Select Conductor GameObject
   - In Inspector, click **Add Component**
   - Search for and add: `Conductor`
   - Set BPM: `120` (or your desired tempo)
   - Enable **Show Debug Info** checkbox

3. **Add Audio (Optional but Recommended):**
   - With Conductor selected, click **Add Component**
   - Search for and add: `Audio Source`
   - Drag a music clip to the **Audio Clip** slot (120 BPM recommended)
   - Check **Loop** checkbox
   - Back in Conductor component, drag AudioSource to **Audio Source** slot

### 3. Create the Player

1. **Create Root GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Name it: `Player`
   - Position: (0, 1, 0) - slightly above ground

2. **Add Physics Components:**
   - Select Player
   - Add Component → `Rigidbody`
     - ✓ Use Gravity
     - Mass: 1
     - Drag: 0
     - Angular Drag: 0.05
     - Constraints: Check **Freeze Rotation X, Y, Z**
   - Add Component → `Capsule Collider`
     - Center: (0, 0, 0)
     - Radius: 0.5
     - Height: 2
     - Direction: Y-Axis

3. **Add PlayerController:**
   - Add Component → `Player Controller`
   - Settings:
     - Move Distance: `2`
     - Move Duration: `0.2`
     - Stumble Duration: `0.5`
     - ✓ Show Debug Info

4. **Add Input System:**
   - Add Component → `Player Input`
   - In Inspector:
     - **Actions**: Click the circle icon → Select `PlayerInputActions`
     - **Default Map**: `Player`
     - **Behavior**: `Send Messages` or `Invoke Unity Events`

5. **Create Visual Sprite (2.5D):**
   - Right-click Player in Hierarchy → 3D Object → Quad
   - Name child: `Sprite`
   - Transform:
     - Position: (0, 0, 0)
     - Rotation: (0, 0, 0)
     - Scale: (1, 2, 1) - makes a vertical rectangle
   - Add Component to Sprite: `Billboard Sprite`
     - ✓ Freeze X Rotation
     - ✓ Freeze Z Rotation

6. **Add Material to Sprite (Visual):**
   - Create a Material: Right-click in Project → Create → Material
   - Name: `PlayerMaterial`
   - Shader: `Unlit/Texture` or `Sprites/Default`
   - Drag a sprite/texture to the Albedo/Texture slot
   - Drag material onto Sprite Quad's Mesh Renderer component

7. **Add Animator (Optional):**
   - If you want visual animations for Move/Stumble:
     - Add Component to Player or Sprite: `Animator`
     - Create an Animator Controller
     - Add triggers: "Move" and "Stumble"
   - Note: Animator is optional - system works without it

### 4. Create Debug UI (Beat Visualizer)

1. **Create Canvas:**
   - Right-click Hierarchy → UI → Canvas
   - Name: `DebugUI`
   - Canvas settings:
     - Render Mode: Screen Space - Overlay
     - Canvas Scaler: Scale With Screen Size (optional)

2. **Create Beat Circle:**
   - Right-click Canvas → UI → Image
   - Name: `BeatCircle`
   - RectTransform:
     - Anchor: Center
     - Position X: 0, Position Y: 0
     - Width: 100, Height: 100
   - Image component:
     - Color: White (or cyan)
     - **Source Image**: None (or UI Sprite circle)
   - Add Component → `Beat Visualizer`
     - Normal Scale: `1`
     - Pulse Scale: `1.5`
     - Pulse Duration: `0.2`
     - ✓ Change Color On Beat
     - Normal Color: White
     - Beat Color: Cyan

3. **Create a Circle Sprite (if needed):**
   - If no circle sprite exists:
   - Right-click in Project → Create → Sprites → Circle
   - Drag to BeatCircle's Source Image slot

### 5. Setup Camera

The scene should already have a Main Camera. Configure it:
1. Select Main Camera
2. Transform:
   - Position: (0, 5, -10)
   - Rotation: (30, 0, 0) - looking down at the player
3. Camera:
   - Field of View: 60
   - Ensure Tag is set to: `MainCamera`

### 6. Create Ground (Optional but Helpful)

1. Right-click Hierarchy → 3D Object → Plane
2. Name: `Ground`
3. Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (2, 1, 2) - 20x20 units
4. Create Material for Ground:
   - Right-click Project → Create → Material
   - Name: `GroundMaterial`
   - Set color to gray or add a grid texture
   - Drag onto Ground

### 7. Add Lighting

The scene should have a Directional Light. Verify:
1. Select Directional Light
2. Transform:
   - Rotation: (50, -30, 0)
3. Light:
   - Type: Directional
   - Intensity: 1
   - Color: Warm white

### 8. Test the Scene

1. **Press Play** in Unity Editor

2. **Watch for:**
   - Debug GUI in upper-left showing BPM, Song Position, Beat Count
   - BeatCircle pulsing in center of screen
   - "Input Window: OPEN/CLOSED" indicator

3. **Test Movement:**
   - Wait for circle to pulse
   - Press **W/A/S/D** RIGHT when it pulses (or just after)
   - Player should dash in that direction
   - Console shows: "Movement SUCCESS! Timing: Perfect/Good/Ok"

4. **Test Off-Beat:**
   - Press **W/A/S/D** between pulses
   - Player should NOT move
   - Console shows: "Movement FAILED! Off-beat input"
   - Player is locked out for 0.5 seconds

5. **Listen for Audio:**
   - Success: Higher-pitched "snare" sound (if assigned)
   - Failure: Lower "wood block" sound (if assigned)

## Troubleshooting

### Player doesn't move at all:
- [ ] Check PlayerInput component has PlayerInputActions assigned
- [ ] Verify Conductor is running (beat count increasing in debug GUI)
- [ ] Check console for errors
- [ ] Ensure Rigidbody is NOT kinematic

### Beat timing feels wrong:
- [ ] Verify Conductor BPM matches your music (if using audio)
- [ ] Check AudioSource is playing and looped
- [ ] Look at "Time to Next Beat" in debug GUI
- [ ] Tolerance is ±0.15s by default (hardcoded in RhythmValidator)

### Sprite doesn't face camera:
- [ ] Ensure BillboardSprite is on the child Quad, NOT the parent Player
- [ ] Check Main Camera exists and has "MainCamera" tag
- [ ] Verify camera is not orthographic (should be perspective)

### Debug UI doesn't pulse:
- [ ] Check BeatVisualizer component is on the UI Image
- [ ] Verify Conductor exists in scene
- [ ] Look for errors in console about event subscription
- [ ] Ensure GameObject is active and enabled

### No audio feedback:
- [ ] AudioClips need to be assigned in PlayerController inspector
- [ ] Create placeholder sounds: Right-click → Create → Audio Clip (won't work, need actual files)
- [ ] For testing, you can skip audio - visual feedback still works

## Next Steps

Once you have the scene working:
1. Experiment with different BPM values (try 60, 90, 120, 140, 180)
2. Adjust tolerance in RhythmValidator.cs if too strict/lenient
3. Add more visual feedback (particles, screen shake, etc.)
4. Create your own sprite art for the player
5. Wait for Phase 3 implementation for pickup/item mechanics

## Quick Reference

### Key Components
- **Conductor**: Global rhythm clock (singleton)
- **PlayerController**: Handles rhythm-validated movement
- **RhythmValidator**: Static helper - checks if input is on-beat
- **BeatVisualizer**: Visual feedback for beat timing
- **BillboardSprite**: Makes 2D sprite face camera

### Default Settings
- BPM: 120
- Timing Window: ±0.15 seconds
- Move Distance: 2 units
- Stumble Duration: 0.5 seconds
- Pulse Duration: 0.2 seconds

### Input Bindings
- **Movement**: WASD or Gamepad Left Stick
- **Interact**: E or Gamepad South Button (not yet used)
- **Action**: Left Mouse or Gamepad Right Trigger (not yet used)
