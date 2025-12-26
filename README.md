# Scribe & Survive - Unity MVP Implementation

## Project Overview
A co-op rhythm-action game where all actions must be performed on beat. Built in Unity 2022.3+ with 2.5D perspective.

## Project Structure
```
Assets/
├── Scripts/           # C# Scripts
│   ├── Conductor.cs           # Global rhythm clock (Singleton)
│   ├── RhythmValidator.cs     # Input timing validation
│   ├── PlayerController.cs    # On-beat movement controller
│   ├── BillboardSprite.cs     # 2.5D sprite billboarding
│   └── BeatVisualizer.cs      # Debug UI beat pulse
├── Scenes/            # Unity scenes
│   └── Gym_Rhythm.unity       # Main test scene
├── Settings/          # Input System configuration
│   └── PlayerInputActions.inputactions
├── Prefabs/           # Reusable GameObjects
├── Audio/             # Sound effects and music
└── UI/                # UI elements
```

## Phase 1: Rhythm Engine (Completed)

### Conductor System
- **Singleton** pattern for global rhythm clock
- Uses `AudioSettings.dspTime` for precision timing
- Default BPM: 120 (configurable)
- Fires `OnBeat` event every quarter note
- Tracks song position and beat count

### RhythmValidator
- Static helper class for input validation
- Default tolerance: ±0.15 seconds
- Returns timing accuracy and grades (Perfect, Good, Ok, Miss)

### Debug UI
- BeatVisualizer component pulses on each beat
- Visual and color feedback
- Helps players internalize rhythm timing

## Phase 2: Player Controller & 2.5D Movement (Completed)

### PlayerController
- **Input**: WASD or Left Stick (Unity New Input System)
- **Validation**: All movement inputs checked with RhythmValidator
- **Success**: Smooth physics-based dash/step movement
- **Failure**: Stumble state (0.5s input lockout)
- **Audio Feedback**: 
  - Success sound (Snare - placeholder)
  - Failure sound (Wood Block - placeholder)

### BillboardSprite
- Attach to child Quad object
- Always faces camera
- Creates 2.5D visual effect

### Physics Setup
- Uses 3D Rigidbody with Capsule Collider
- Gravity enabled
- Rotation constraints for upright character

## Setup Instructions

### Unity Project Setup
1. Open Unity Hub
2. Create new project with Unity 2022.3+ (LTS)
3. Clone this repository or copy the Assets folder
4. Unity will automatically import the New Input System package

### Scene Setup: Gym_Rhythm

#### 1. Create Conductor GameObject
1. Create empty GameObject named "Conductor"
2. Add `Conductor.cs` component
3. Set BPM to 120 (or desired tempo)
4. (Optional) Add AudioSource with looping music

#### 2. Create Player GameObject
1. Create empty GameObject named "Player"
2. Add `PlayerController.cs` component
3. Add `PlayerInput` component
   - Set "Actions" to PlayerInputActions asset
   - Set "Default Map" to "Player"
4. Add `Rigidbody` component
   - Check "Use Gravity"
   - Set Constraints: Freeze Rotation (X, Y, Z)
5. Add `Capsule Collider` (height: 2, radius: 0.5)
6. Create child GameObject named "Sprite"
   - Add Quad mesh (scale: 1, 2, 1)
   - Add `BillboardSprite.cs` component
   - Add material/sprite texture

#### 3. Create Debug UI
1. Create Canvas (Screen Space - Overlay)
2. Create UI Image (Circle)
   - Position: Center of screen
   - Size: 100x100
   - Color: White
3. Add `BeatVisualizer.cs` component to Image
   - Set Pulse Scale: 1.5
   - Set Pulse Duration: 0.2s
   - Enable Change Color On Beat

#### 4. Setup Camera
1. Position camera to view player (e.g., position: 0, 5, -10, rotation: 30, 0, 0)

## Testing the Implementation

### Rhythm Timing Test
1. Press Play in Unity
2. Watch the Beat Visualizer pulse
3. Observe the debug GUI showing:
   - Current BPM and song position
   - Beat count
   - Time to next beat
   - Player state
   - Input window (GREEN = on beat, RED = off beat)

### Movement Test
1. Try pressing WASD on the beat (when visualizer pulses)
   - Player should move smoothly
   - Success sound should play
   - Debug log shows "SUCCESS"
2. Try pressing WASD off the beat
   - Player should NOT move
   - Failure sound should play
   - Player enters stumble state (0.5s lockout)
   - Debug log shows "FAILED"

### Expected Behavior
- **On Beat (±0.15s)**: Smooth dash movement in input direction
- **Off Beat**: No movement, audio feedback, temporary lockout
- **Visual Feedback**: Circle pulses on every beat
- **Debug Info**: Real-time rhythm data in upper-left corner

## Technical Architecture

### Rhythm Precision
- Uses `AudioSettings.dspTime` (NOT `Time.time` or `Update()`)
- Beat detection based on DSP timing
- Input validation checks distance to nearest beat

### Movement System
- Physics-based using `Rigidbody.MovePosition()`
- Smooth interpolation with AnimationCurve
- Fixed distance dash (configurable)
- State machine prevents overlapping movements

### 2.5D Rendering
- 3D physics (Capsule Collider, Rigidbody)
- 2D sprite on billboard Quad
- Sprite always faces camera

## Configuration

### Conductor Settings
- **BPM**: 120 (adjustable, affects all rhythm timing)
- **Audio Source**: Optional background music loop

### PlayerController Settings
- **Move Distance**: 2.0 units (dash length)
- **Move Duration**: 0.2 seconds (dash animation time)
- **Stumble Duration**: 0.5 seconds (input lockout)
- **Move Curve**: AnimationCurve for movement smoothing

### RhythmValidator Settings
- **Tolerance**: ±0.15 seconds (hardcoded, can be parameterized)

## Next Steps (Phase 3)

Phase 3 will implement:
- `IHoldable` interface for items
- `PlayerInventory` system
- Pickup/Drop mechanics
- "Butterfingers" rule (drop items on stumble)
- Fluid class system based on held item

**Wait for confirmation before proceeding to Phase 3.**

## Debugging Tips

### If rhythm feels off:
1. Check Conductor has correct BPM
2. Verify AudioSource is playing (if using music)
3. Watch debug GUI for timing values
4. Adjust tolerance in RhythmValidator if needed

### If player doesn't move:
1. Check PlayerInput component is set up correctly
2. Verify Input Actions asset is assigned
3. Check console for "Movement SUCCESS" logs
4. Ensure Rigidbody isn't kinematic

### If sprite doesn't face camera:
1. Verify BillboardSprite is on child Quad object
2. Check Main Camera is tagged correctly
3. Ensure camera exists in scene

## Code Style Notes
- Modular, composition-based design
- Strict typing throughout
- Events for decoupled communication
- Coroutines for state timing
- No deep inheritance trees
