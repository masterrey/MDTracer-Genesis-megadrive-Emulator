# Basic Emulator Sample

This sample demonstrates how to set up and use the Genesis Emulator in Unity.

## Contents

- **Scenes/BasicEmulator.unity**: A simple scene with the emulator set up
- **Scripts/EmulatorUI.cs**: Example UI controller script
- **Prefabs/GenesisEmulatorCanvas.prefab**: Pre-configured UI setup

## Setup Instructions

1. Import this sample into your project via Package Manager
2. Place your Genesis ROM file in `Assets/StreamingAssets/`
3. Open the BasicEmulator scene
4. In the GenesisEmulator component on the "Emulator" GameObject:
   - Set the ROM Path to your ROM filename
   - Enable Auto Load
5. Press Play

## Controls

- Arrow Keys / WASD - D-Pad
- Z / J - Button A  
- X / K - Button B
- C / L - Button C
- Enter - Start
- P - Pause/Resume
- R - Reset
- Escape - Stop

## Customization

You can customize the emulator by modifying the GenesisEmulator component properties:

- Adjust Render Scale for different output sizes
- Toggle Show Debug Info to see emulation statistics
- Modify Audio Volume to control sound levels

## Note

You must provide your own ROM files. The emulator does not include any games.
