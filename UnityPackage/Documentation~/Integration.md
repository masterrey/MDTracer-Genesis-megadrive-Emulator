# Integration Guide

This document explains how the original MDTracer emulator has been adapted for Unity.

## Architecture Overview

The Unity package follows a layered architecture:

```
┌─────────────────────────────────────┐
│   Unity MonoBehaviour Layer         │
│   (GenesisEmulator.cs)              │
├─────────────────────────────────────┤
│   Core Emulation Layer              │
│   (GenesisCore.cs)                  │
├─────────────────────────────────────┤
│   Hardware Components               │
│   - M68K CPU                        │
│   - Z80 CPU                         │
│   - VDP (Graphics)                  │
│   - YM2612 (FM Sound)               │
│   - SN76489 (PSG Sound)             │
│   - Memory Bus                      │
│   - Cartridge                       │
└─────────────────────────────────────┘
```

## Key Differences from Original

### 1. No Windows Forms Dependencies

The original MDTracer uses Windows Forms for its UI. The Unity version:
- Removes all `System.Windows.Forms` references
- Replaces `MessageBox` calls with Unity's `Debug.Log`
- Uses Unity's component system instead of Form classes

### 2. Rendering

**Original:** Uses SharpDX (DirectX 12) for rendering
**Unity:** Uses Unity's Texture2D and RenderTexture

```csharp
// Original: SharpDX rendering
// Unity: Simple texture-based rendering
frameBuffer.SetPixels32(pixels);
frameBuffer.Apply();
Graphics.Blit(frameBuffer, targetTexture);
```

### 3. Audio

**Original:** Uses NAudio for audio output
**Unity:** Will use Unity's AudioSource and OnAudioFilterRead

```csharp
// Future implementation:
void OnAudioFilterRead(float[] data, int channels)
{
    emulatorCore.GetAudioSamples(data);
}
```

### 4. Input

**Original:** Uses SharpDX.DirectInput for gamepad support
**Unity:** Uses Unity's Input system

```csharp
// Unity input mapping
input.Up = Input.GetKey(KeyCode.UpArrow);
input.ButtonA = Input.GetKey(KeyCode.Z);
```

## Integration Strategy

### Phase 1: Package Structure (✅ Complete)

- Created Unity package structure
- Set up assembly definitions
- Created package.json and README
- Implemented stub classes for all components

### Phase 2: Core Component Integration (⚠️ In Progress)

The following components from the original MDTracer need to be integrated:

#### M68K CPU (md_m68k.cs and related files)
- `md_m68k.cs` - Main CPU logic
- `md_m68k_addressing.cs` - Addressing modes
- `md_m68k_memory.cs` - Memory access
- `md_m68k_sub.cs` - Helper functions
- `opc/` folder - Opcode implementations

**Integration steps:**
1. Copy the original files to `UnityPackage/Runtime/Core/CPU/M68K/`
2. Update namespace to `MDTracer.Unity`
3. Replace `MessageBox.Show` with `Debug.LogError`
4. Remove Windows Forms dependencies
5. Connect to MemoryBus

#### Z80 CPU (md_z80.cs and related files)
- `md_z80.cs` - Main Z80 logic
- `md_z80_memory.cs` - Memory access
- `md_z80_operand.cs` - Operand handling

**Integration steps:**
1. Copy files to `UnityPackage/Runtime/Core/CPU/Z80/`
2. Update namespace and remove dependencies
3. Connect to MemoryBus

#### VDP (md_vdp.cs and related files)
- `md_vdp.cs` - Main VDP logic
- `md_vdp_renderer.cs` - Rendering engine
- `md_vdp_renderer_line.cs` - Line rendering
- `md_vdp_renderer_data.cs` - Sprite/tile data
- `md_vdp_memory.cs` - VRAM access
- `md_vdp_regster.cs` - VDP registers

**Integration steps:**
1. Copy files to `UnityPackage/Runtime/Core/VDP/`
2. Replace DirectX rendering with Color32 array output
3. Remove shader compilation (md_vdp_renderer_directx_update.hlsl)
4. Simplify rendering to output RGB pixels

#### Sound Chips
- `md_music_ym2612_core.cs` - YM2612 FM synthesis
- `md_music_sn76489_core.cs` - SN76489 PSG

**Integration steps:**
1. Copy files to `UnityPackage/Runtime/Core/Sound/`
2. Adapt audio output for Unity's audio system
3. Remove NAudio dependencies

### Phase 3: Full Integration Testing

Once core components are integrated:
1. Test with actual ROM files
2. Verify CPU execution
3. Verify graphics rendering
4. Verify sound output
5. Verify input handling

## File Mapping

| Original File | Unity Location | Status |
|--------------|----------------|--------|
| md_m68k.cs | Runtime/Core/M68KProcessor.cs | Stub |
| md_z80.cs | Runtime/Core/Z80Processor.cs | Stub |
| md_vdp.cs | Runtime/Core/VDPChip.cs | Stub |
| md_music_ym2612_core.cs | Runtime/Core/YM2612SoundChip.cs | Stub |
| md_music_sn76489_core.cs | Runtime/Core/SN76489SoundChip.cs | Stub |
| md_cartridge.cs | Runtime/Core/CartridgeData.cs | Partial |
| md_bus.cs | Runtime/Core/MemoryBus.cs | Partial |

## Next Steps

To complete the integration:

1. **Copy Original Core Files**
   ```bash
   # Copy CPU emulation
   cp MDTracer/md_m68k*.cs UnityPackage/Runtime/Core/CPU/M68K/
   cp MDTracer/md_z80*.cs UnityPackage/Runtime/Core/CPU/Z80/
   
   # Copy VDP
   cp MDTracer/md_vdp*.cs UnityPackage/Runtime/Core/VDP/
   
   # Copy Sound
   cp MDTracer/md_music*.cs UnityPackage/Runtime/Core/Sound/
   ```

2. **Update Namespaces**
   - Replace `namespace MDTracer` with `namespace MDTracer.Unity`
   - Add `using UnityEngine;` where needed

3. **Remove Dependencies**
   - Replace `MessageBox.Show()` with `Debug.LogError()`
   - Remove `using System.Windows.Forms`
   - Replace DirectX code with Unity equivalents

4. **Test Integration**
   - Create test scenes with sample ROMs
   - Verify emulation accuracy
   - Performance testing

## Notes for Developers

- **Unsafe Code**: The emulator uses `unsafe` code blocks for performance. Make sure `allowUnsafeCode` is enabled in the .asmdef file.

- **Performance**: The original emulator is optimized for desktop. You may need to add frame-skipping or other optimizations for mobile platforms.

- **Threading**: The original uses multiple threads. Unity's threading restrictions mean we need to be careful with threading in the emulation loop.

- **ROM Files**: Never include ROM files in the package. Users must provide their own legally obtained ROMs.

## Legal Considerations

This package is for educational and research purposes. Users must:
- Own legal copies of any ROM files they use
- Not distribute copyrighted ROM files
- Respect intellectual property rights

The emulator itself is licensed under MIT, but ROM files are copyrighted by their respective owners.
