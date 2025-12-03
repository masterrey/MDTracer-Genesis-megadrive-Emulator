# Unity Integration - Quick Start

This document provides a quick overview of the Unity package integration for the MDTracer Genesis/MegaDrive emulator.

## What's Been Done

The repository now includes a complete Unity package structure that allows the emulator to be used in Unity projects:

### ✅ Completed Features

1. **Package Structure**
   - Unity Package Manager compatible (`package.json`)
   - Proper assembly definitions for code organization
   - MIT license compatibility maintained

2. **Core Architecture**
   - `GenesisEmulator` MonoBehaviour component - main Unity interface
   - `GenesisCore` - emulator engine coordination
   - Stub implementations for all hardware components:
     - M68K CPU (main processor)
     - Z80 CPU (sound processor)
     - VDP (graphics chip)
     - YM2612 (FM sound chip)
     - SN76489 (PSG sound chip)
     - Memory bus system
     - Cartridge ROM loading

3. **Unity Integration**
   - Input handling (keyboard controls)
   - Rendering output (Texture2D/RenderTexture)
   - ROM loading from StreamingAssets
   - Custom Inspector editor for better UX

4. **Documentation**
   - Comprehensive README with usage instructions
   - Integration guide explaining architecture
   - Implementation guide for completing the integration
   - Sample code and examples
   - CHANGELOG tracking progress

### ⚠️ In Progress (Needs Integration)

The core emulation logic from the original MDTracer needs to be integrated:

1. **CPU Emulation**
   - Full M68K instruction set (from `md_m68k.cs` + opcodes)
   - Full Z80 instruction set (from `md_z80.cs`)

2. **Graphics Rendering**
   - Complete VDP implementation (from `md_vdp.cs` and related files)
   - Sprite rendering
   - Tile/background rendering
   - Color palette management

3. **Sound Emulation**
   - YM2612 FM synthesis (from `md_music_ym2612_core.cs`)
   - SN76489 PSG (from `md_music_sn76489_core.cs`)
   - Audio mixing and output

## Installation in Unity

### Method 1: Via Git URL (Recommended)

1. Open Unity (2020.3 or newer)
2. Go to Window > Package Manager
3. Click '+' > "Add package from git URL"
4. Enter: `https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator.git?path=/UnityPackage`

### Method 2: Local Installation

1. Clone this repository
2. Copy the `UnityPackage` folder to your project's `Packages` folder
3. Unity will automatically detect and import it

## Basic Usage

1. **Create GameObject with Emulator:**
   ```
   GameObject > Create Empty
   Add Component > MDTracer > Genesis Emulator
   ```

2. **Setup Display:**
   - Create UI > Raw Image in Canvas
   - Assign the Raw Image to emulator's "Target Image"

3. **Load ROM:**
   - Place ROM file in `Assets/StreamingAssets/`
   - Set "Rom Path" in emulator component
   - Enable "Auto Load"
   - Press Play

## Example Code

```csharp
using MDTracer.Unity;
using UnityEngine;

public class MyEmulatorController : MonoBehaviour
{
    public GenesisEmulator emulator;
    
    void Start()
    {
        // Load a ROM
        if (emulator.LoadROM("sonic.bin"))
        {
            Debug.Log("ROM loaded successfully!");
        }
    }
    
    void Update()
    {
        // Control emulator with keyboard
        if (Input.GetKeyDown(KeyCode.P))
            emulator.Pause();
        
        if (Input.GetKeyDown(KeyCode.R))
            emulator.Reset();
    }
}
```

## Current Limitations

⚠️ **Important:** The current version includes stub implementations of the emulation cores. It will not run actual ROM files until the full emulation logic is integrated from the original codebase.

To run actual games, the following needs to be completed:
- Integrate M68K CPU emulation (see `Documentation~/ImplementationGuide.md`)
- Integrate VDP graphics rendering
- Integrate sound chip emulation

## Directory Structure

```
UnityPackage/
├── package.json              # Package manifest
├── README.md                 # User documentation
├── CHANGELOG.md             # Version history
├── LICENSE.md               # License information
├── Runtime/
│   ├── MDTracer.Runtime.asmdef
│   ├── Components/
│   │   └── GenesisEmulator.cs      # Main Unity component
│   └── Core/
│       ├── GenesisCore.cs          # Emulator coordinator
│       ├── M68KProcessor.cs        # CPU (stub)
│       ├── Z80Processor.cs         # Sound CPU (stub)
│       ├── VDPChip.cs              # Graphics (stub)
│       ├── YM2612SoundChip.cs      # FM sound (stub)
│       ├── SN76489SoundChip.cs     # PSG sound (stub)
│       ├── MemoryBus.cs            # Memory mapping
│       └── CartridgeData.cs        # ROM loading
├── Editor/
│   ├── MDTracer.Editor.asmdef
│   └── GenesisEmulatorEditor.cs    # Custom inspector
├── Documentation~/
│   ├── Integration.md              # Architecture overview
│   └── ImplementationGuide.md      # Integration instructions
└── Samples~/
    └── BasicEmulator/              # Example scene
        ├── README.md
        └── Scripts/
            └── EmulatorUI.cs       # Example UI controller
```

## Contributing

To complete the emulator integration:

1. Read `UnityPackage/Documentation~/ImplementationGuide.md`
2. Follow the step-by-step integration process
3. Copy and adapt code from the original `MDTracer/` folder
4. Remove Windows Forms dependencies
5. Replace DirectX rendering with Unity equivalents
6. Test with actual ROM files

## Technical Details

### Rendering Pipeline
```
VDP Emulation → Color32[] Array → Texture2D → RenderTexture → UI RawImage
```

### Audio Pipeline (Planned)
```
YM2612 + SN76489 → Audio Mixer → OnAudioFilterRead → Unity AudioSource
```

### Input Pipeline
```
Unity Input System → GenesisInput Struct → Memory Bus → CPU
```

## Resources

- **Original MDTracer**: https://github.com/sasayaki-japan/MDTracer
- **Package Documentation**: `UnityPackage/README.md`
- **Implementation Guide**: `UnityPackage/Documentation~/ImplementationGuide.md`
- **Sample Code**: `UnityPackage/Samples~/BasicEmulator/`

## License

This Unity package maintains the MIT license of the original MDTracer project. See LICENSE file for details.

## Support

For issues or questions:
- GitHub Issues: https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator/issues
- Original Project: https://www.jppass.jp/mdtracer

---

**Status**: Package structure complete, core integration in progress.

**Next Steps**: Integrate CPU, VDP, and sound chip emulation from original codebase.
