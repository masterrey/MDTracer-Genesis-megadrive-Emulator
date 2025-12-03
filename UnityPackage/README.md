# MDTracer Genesis Emulator for Unity

A SEGA Genesis/MegaDrive emulator package for Unity. This package allows you to run Genesis ROM files directly in your Unity projects.

## Features

- Full Genesis/MegaDrive emulation in Unity
- Easy-to-use MonoBehaviour component
- Keyboard input support
- Configurable rendering and audio
- Compatible with Unity 2020.3 and newer

## Installation

### Via Unity Package Manager (Git URL)

1. Open Unity Package Manager (Window > Package Manager)
2. Click the '+' button and select "Add package from git URL"
3. Enter the repository URL: `https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator.git?path=/UnityPackage`

### Manual Installation

1. Clone or download this repository
2. Copy the `UnityPackage` folder to your Unity project's `Packages` folder
3. Unity will automatically detect and import the package

## Quick Start

### 1. Setup the Scene

1. Create a new GameObject in your scene
2. Add the `GenesisEmulator` component to it (Add Component > MDTracer > Genesis Emulator)
3. Create a UI RawImage for displaying the emulator output
4. Assign the RawImage to the `Target Image` field in the GenesisEmulator component

### 2. Load a ROM

#### Option A: Automatic Loading

1. Place your ROM file in the `Assets/StreamingAssets` folder
2. In the GenesisEmulator component:
   - Set `Rom Path` to your ROM filename (e.g., "sonic.bin")
   - Enable `Auto Load`
3. Press Play

#### Option B: Manual Loading via Script

```csharp
using MDTracer.Unity;
using UnityEngine;

public class EmulatorController : MonoBehaviour
{
    public GenesisEmulator emulator;
    
    void Start()
    {
        // Load ROM from StreamingAssets
        emulator.LoadROM("sonic.bin");
        
        // Or load from absolute path
        // emulator.LoadROM("/path/to/rom.bin");
    }
}
```

### 3. Controls

Default keyboard controls:

- **D-Pad**: Arrow Keys or WASD
- **Button A**: Z or J
- **Button B**: X or K
- **Button C**: C or L
- **Start**: Enter

### 4. Controlling the Emulator

```csharp
using MDTracer.Unity;
using UnityEngine;

public class EmulatorController : MonoBehaviour
{
    public GenesisEmulator emulator;
    
    public void PauseEmulator()
    {
        emulator.Pause();
    }
    
    public void ResumeEmulator()
    {
        emulator.Resume();
    }
    
    public void ResetEmulator()
    {
        emulator.Reset();
    }
    
    public void StopEmulator()
    {
        emulator.Stop();
    }
}
```

## Component Properties

### ROM Settings

- **Rom Path**: Path to the ROM file (relative to StreamingAssets or absolute)
- **Auto Load**: Load ROM automatically when the scene starts

### Display Settings

- **Target Texture**: RenderTexture to render the emulator output
- **Target Image**: UI RawImage to display the emulator output
- **Render Scale**: Scaling factor for the output (1.0 = 320x224 native resolution)

### Emulation Settings

- **Target Frame Rate**: Target frame rate (Genesis runs at ~60 FPS)
- **Enable Audio**: Enable/disable audio output
- **Audio Volume**: Audio volume (0.0 to 1.0)

### Debug

- **Show Debug Info**: Display debug information on screen

## Example Scene

A sample scene is included in the package under `Samples~/BasicEmulator/`. To import:

1. Open Unity Package Manager
2. Find "MDTracer Genesis Emulator" in the package list
3. Expand "Samples"
4. Click "Import" next to "Basic Emulator"

## System Requirements

- Unity 2020.3 or newer
- .NET Standard 2.0 or .NET 4.x
- Valid Genesis/MegaDrive ROM files (not included)

## Current Status

This is a Unity port of the MDTracer emulator. The core emulation components are being integrated from the original C# codebase.

### Implemented Features

- ✅ Package structure
- ✅ Unity component interface
- ✅ ROM loading
- ✅ Input handling
- ✅ Display output
- ✅ Core architecture stubs

### In Progress

- ⚠️ M68K CPU emulation (integration from original codebase)
- ⚠️ Z80 CPU emulation (integration from original codebase)
- ⚠️ VDP graphics rendering (integration from original codebase)
- ⚠️ YM2612 sound emulation (integration from original codebase)
- ⚠️ SN76489 PSG emulation (integration from original codebase)

## Compatibility

The emulator aims to be compatible with the same games as the original MDTracer project. See the main repository README for a list of verified titles.

## Legal Notice

This emulator is provided for educational and research purposes. You must own legal copies of any ROM files you use with this emulator. The authors do not condone or support software piracy.

## License

This project is licensed under the MIT License - see the LICENSE file in the root of the repository for details.

## Credits

Based on the [MDTracer](https://github.com/sasayaki-japan/MDTracer) project by sasayaki-japan.

Unity integration by the MDTracer community.

## Support

For issues, questions, or contributions, please visit:
https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator

## References

- Original MDTracer: https://www.jppass.jp/mdtracer
- Genesis Technical Overview
- MC68000 User's Manual
- Z80 CPU User Manual
- YM2612 Documentation
- SN76489 User Manual
