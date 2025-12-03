# Implementation Guide

This guide provides detailed instructions for completing the emulator integration from the original MDTracer codebase.

## Current Status

The Unity package currently has:
- ✅ Complete package structure
- ✅ Unity MonoBehaviour wrapper (GenesisEmulator)
- ✅ Stub implementations for all core components
- ✅ ROM loading and header parsing
- ✅ Input handling
- ✅ Rendering pipeline (outputs to Unity textures)
- ⚠️ CPU emulation (stubs only - needs integration)
- ⚠️ Graphics rendering (stubs only - needs integration)
- ⚠️ Sound emulation (stubs only - needs integration)

## Integration Steps

### Step 1: Integrate M68K CPU Emulation

The M68K (Motorola 68000) CPU is the main processor of the Genesis/MegaDrive.

#### Files to Integrate

From `MDTracer/` directory:
- `md_m68k.cs` - Main CPU class
- `md_m68k_addressing.cs` - Addressing mode implementations
- `md_m68k_initialize.cs` - Initialization code
- `md_m68k_initialize2.cs` - Additional initialization
- `md_m68k_memory.cs` - Memory access methods
- `md_m68k_sub.cs` - Helper functions
- `opc/` directory - All opcode implementation files

#### Integration Process

1. **Create CPU directory structure:**
   ```bash
   mkdir -p UnityPackage/Runtime/Core/CPU/M68K/Opcodes
   ```

2. **Copy files:**
   ```bash
   cp MDTracer/md_m68k*.cs UnityPackage/Runtime/Core/CPU/M68K/
   cp MDTracer/opc/*.cs UnityPackage/Runtime/Core/CPU/M68K/Opcodes/
   ```

3. **Update namespaces:**
   Replace all instances of:
   ```csharp
   namespace MDTracer
   ```
   With:
   ```csharp
   namespace MDTracer.Unity
   ```

4. **Remove Windows Forms dependencies:**
   Find and replace:
   ```csharp
   // Remove this
   using System.Windows.Forms;
   
   // Replace MessageBox.Show calls with Debug.LogError
   MessageBox.Show("Error message", "Title");
   // Becomes:
   UnityEngine.Debug.LogError("Error message");
   ```

5. **Update M68KProcessor.cs stub:**
   Replace the stub implementation with a wrapper that uses the integrated md_m68k class:
   ```csharp
   public class M68KProcessor : IDisposable
   {
       private md_m68k cpuCore;
       
       public M68KProcessor()
       {
           cpuCore = new md_m68k();
       }
       
       public void SetMemoryBus(MemoryBus bus)
       {
           // Connect cpu to memory bus
       }
       
       public void Reset()
       {
           cpuCore.reset();
       }
       
       public int ExecuteInstruction()
       {
           return cpuCore.execute_one();
       }
   }
   ```

6. **Update memory access:**
   Ensure the M68K accesses memory through the MemoryBus:
   ```csharp
   // In md_m68k memory access methods, call MemoryBus methods
   byte value = memory.ReadByte(address);
   ```

### Step 2: Integrate Z80 CPU Emulation

The Z80 is used for sound processing in the Genesis/MegaDrive.

#### Files to Integrate

From `MDTracer/` directory:
- `md_z80.cs` - Main Z80 CPU
- `md_z80_initialize.cs` - Initialization
- `md_z80_memory.cs` - Memory access
- `md_z80_operand.cs` - Operand handling
- `md_z80_operand_2.cs` - Additional operands
- `md_z80_operand_sub.cs` - Operand helpers

#### Integration Process

Similar to M68K:
1. Create directory: `UnityPackage/Runtime/Core/CPU/Z80/`
2. Copy files
3. Update namespaces
4. Remove Windows Forms dependencies
5. Update Z80Processor.cs stub to use the integrated md_z80 class

### Step 3: Integrate VDP (Video Display Processor)

The VDP handles all graphics rendering.

#### Files to Integrate

From `MDTracer/` directory:
- `md_vdp.cs` - Main VDP
- `md_vdp_renderer.cs` - Rendering engine
- `md_vdp_renderer_line.cs` - Scanline rendering
- `md_vdp_renderer_data.cs` - Sprite/tile data
- `md_vdp_renderer_snap.cs` - Screenshot/snapshot
- `md_vdp_memory.cs` - VRAM access
- `md_vdp_regster.cs` - VDP registers
- `md_vdp_initialize.cs` - Initialization
- `md_vdp_dma.cs` - DMA operations

#### Key Changes Required

The original uses SharpDX for rendering. For Unity:

1. **Remove DirectX dependencies:**
   - Remove `md_vdp_renderer_frame_directx.cs`
   - Remove `md_vdp_renderer_frame_directx_sub.cs`
   - Remove `md_vdp_renderer_directx_update.hlsl`

2. **Replace rendering output:**
   
   Original (DirectX):
   ```csharp
   // Renders to DirectX texture
   ```
   
   Unity version:
   ```csharp
   // In md_vdp_renderer.cs, modify to output Color32 array
   public void RenderScanline(int scanline, Color32[] frameBuffer)
   {
       // Render pixels to frameBuffer array
       for (int x = 0; x < 320; x++)
       {
           int index = scanline * 320 + x;
           frameBuffer[index] = GetPixelColor(x, scanline);
       }
   }
   ```

3. **Color conversion:**
   Genesis uses 9-bit color (3 bits per channel). Convert to Unity Color32:
   ```csharp
   private Color32 ConvertGenesisColor(ushort genesisColor)
   {
       // Genesis: ---BBBBGGGGRRRR (4 bits per channel, but only 3 used)
       byte r = (byte)(((genesisColor >> 0) & 0x0E) * 255 / 14);
       byte g = (byte)(((genesisColor >> 4) & 0x0E) * 255 / 14);
       byte b = (byte)(((genesisColor >> 8) & 0x0E) * 255 / 14);
       return new Color32(r, g, b, 255);
   }
   ```

4. **Update VDPChip.cs:**
   Replace stub with wrapper for integrated md_vdp:
   ```csharp
   public class VDPChip : IDisposable
   {
       private md_vdp vdpCore;
       private Color32[] frameBuffer;
       
       public void RenderFrame()
       {
           vdpCore.render_frame(frameBuffer);
       }
   }
   ```

### Step 4: Integrate Sound Chips

#### YM2612 (FM Synthesis)

Files from `MDTracer/`:
- `md_music_ym2612_core.cs` - Core YM2612 emulation
- `md_music_ym2612_init.cs` - Initialization
- `md_music_ym2612_regster.cs` - Register handling
- `md_music_ym2612_document.cs` - Documentation

Integration:
1. Copy to `UnityPackage/Runtime/Core/Sound/YM2612/`
2. Remove NAudio dependencies
3. Implement Unity audio output:
   ```csharp
   public void GetAudioSamples(float[] buffer, int sampleRate)
   {
       // Generate samples for the requested buffer size
       for (int i = 0; i < buffer.Length; i++)
       {
           buffer[i] = GenerateNextSample();
       }
   }
   ```

#### SN76489 (PSG)

Files from `MDTracer/`:
- `md_music_sn76489_core.cs` - Core PSG emulation
- `md_music_sn76489_register.cs` - Register handling

Integration similar to YM2612.

### Step 5: Update Memory Bus

The MemoryBus needs to properly connect all components.

From `MDTracer/`:
- `md_bus.cs` - Memory bus implementation
- `md_io.cs` - I/O port handling
- `md_io_device.cs` - I/O devices

Integration:
1. Merge functionality into `UnityPackage/Runtime/Core/MemoryBus.cs`
2. Ensure proper memory mapping:
   - 0x000000-0x3FFFFF: ROM
   - 0xA00000-0xA0FFFF: Z80 address space
   - 0xA10000-0xA10FFF: I/O ports
   - 0xA11000-0xA11FFF: Control registers
   - 0xC00000-0xC0001F: VDP ports
   - 0xFF0000-0xFFFFFF: Work RAM

### Step 6: Add Unity Audio Output

Implement proper audio output using Unity's audio system:

```csharp
public class GenesisEmulator : MonoBehaviour
{
    private AudioSource audioSource;
    private float[] audioBuffer;
    
    void Start()
    {
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
    }
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isRunning || !enableAudio)
            return;
        
        // Get audio samples from emulator
        emulatorCore.GetAudioSamples(data);
        
        // Apply volume
        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= audioVolume;
        }
    }
}
```

### Step 7: Testing

1. **Create test scene:**
   - Add GenesisEmulator component
   - Configure with test ROM
   - Add UI for controls

2. **Verify CPU execution:**
   - Check that CPU fetches and executes instructions
   - Verify program counter updates correctly
   - Test various opcodes

3. **Verify graphics:**
   - Check that VDP renders frames
   - Verify sprite rendering
   - Test scrolling

4. **Verify audio:**
   - Check that sound chips generate audio
   - Verify audio mixing
   - Test different sound effects

5. **Performance testing:**
   - Aim for 60 FPS (Genesis native framerate)
   - Profile CPU usage
   - Optimize hot paths if needed

## Common Issues and Solutions

### Issue: Unsafe code errors

**Solution:** Ensure `allowUnsafeCode` is enabled in assembly definition:
```json
{
    "allowUnsafeCode": true
}
```

### Issue: Threading issues

**Solution:** The original uses multiple threads. Unity has threading restrictions:
- Run emulation on main thread
- Or use Unity's job system
- Be careful with Unity API calls from other threads

### Issue: Performance problems

**Solution:**
- Use Unity Profiler to identify bottlenecks
- Consider using Burst compiler for hot paths
- Implement frame skipping option
- Cache frequently accessed data

### Issue: ROM compatibility

**Solution:**
- Test with verified working ROMs
- Check ROM header parsing
- Verify memory mapping
- Add logging for unknown operations

## Validation Checklist

Before considering integration complete:

- [ ] CPU executes instructions correctly
- [ ] Memory mapping works for all regions
- [ ] VDP renders graphics correctly
- [ ] Sprites display properly
- [ ] Scrolling works
- [ ] Sound chips generate audio
- [ ] Audio mixing works
- [ ] Input controls are responsive
- [ ] Frame rate is stable at ~60 FPS
- [ ] At least one commercial game runs correctly
- [ ] No memory leaks
- [ ] No threading issues
- [ ] Documentation is updated
- [ ] Sample scene works

## Resources

- Genesis Technical Overview: Hardware specifications
- MC68000 User Manual: CPU instruction set
- Z80 User Manual: Sound CPU instructions
- VDP Documentation by Charles MacDonald: Graphics chip details
- YM2612 Manual: FM synthesis chip
- SN76489 Manual: PSG chip

## Getting Help

If you encounter issues during integration:
1. Check the original MDTracer source code
2. Refer to Genesis hardware documentation
3. Look at other emulator implementations (Gens, BlastEm)
4. Ask in the project's GitHub issues

## Next Steps After Integration

Once basic integration is complete:
1. Add save state support
2. Implement configuration options
3. Add debugging features
4. Support for special chips (SVP, etc.)
5. Mobile platform optimization
6. Network multiplayer support
