using System;
using UnityEngine;

namespace MDTracer.Unity
{
    /// <summary>
    /// Core emulation engine for Genesis/MegaDrive.
    /// This class manages the CPU, VDP, sound chips, and memory.
    /// </summary>
    public class GenesisCore : IDisposable
    {
        // Emulator components
        private M68KProcessor cpu;
        private Z80Processor z80;
        private VDPChip vdp;
        private YM2612SoundChip ym2612;
        private SN76489SoundChip psg;
        private MemoryBus memory;
        private CartridgeData cartridge;
        
        // Input state
        private GenesisInput inputState;
        
        // Frame buffer
        private Color32[] frameBuffer;
        private const int SCREEN_WIDTH = 320;
        private const int SCREEN_HEIGHT = 224;
        
        // Timing
        private const int CPU_CLOCK_HZ = 7670453;  // MC68000 clock speed
        private const int FRAME_RATE = 60;
        private const int CYCLES_PER_FRAME = CPU_CLOCK_HZ / FRAME_RATE;
        
        private bool isInitialized = false;
        private bool romLoaded = false;
        
        public GenesisCore()
        {
            frameBuffer = new Color32[SCREEN_WIDTH * SCREEN_HEIGHT];
        }
        
        /// <summary>
        /// Initialize the emulator components.
        /// </summary>
        public void Initialize()
        {
            if (isInitialized)
                return;
            
            // Initialize components
            cpu = new M68KProcessor();
            z80 = new Z80Processor();
            vdp = new VDPChip();
            ym2612 = new YM2612SoundChip();
            psg = new SN76489SoundChip();
            memory = new MemoryBus();
            cartridge = new CartridgeData();
            
            // Connect components
            memory.SetCartridge(cartridge);
            memory.SetVDP(vdp);
            memory.SetZ80(z80);
            memory.SetSoundChips(ym2612, psg);
            
            cpu.SetMemoryBus(memory);
            z80.SetMemoryBus(memory);
            
            vdp.Initialize(SCREEN_WIDTH, SCREEN_HEIGHT);
            
            isInitialized = true;
            
            Debug.Log("Genesis core initialized");
        }
        
        /// <summary>
        /// Load a ROM into the emulator.
        /// </summary>
        public bool LoadROM(byte[] romData)
        {
            if (!isInitialized)
            {
                Debug.LogError("Core not initialized");
                return false;
            }
            
            if (romData == null || romData.Length == 0)
            {
                Debug.LogError("Invalid ROM data");
                return false;
            }
            
            try
            {
                // Load cartridge
                if (!cartridge.LoadROM(romData))
                {
                    Debug.LogError("Failed to load cartridge");
                    return false;
                }
                
                // Reset the system
                Reset();
                
                romLoaded = true;
                
                Debug.Log($"ROM loaded: {cartridge.GameTitle}");
                Debug.Log($"System: {cartridge.SystemType}");
                Debug.Log($"Region: {cartridge.Region}");
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading ROM: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Reset the emulator to initial state.
        /// </summary>
        public void Reset()
        {
            if (!isInitialized)
                return;
            
            cpu.Reset();
            z80.Reset();
            vdp.Reset();
            ym2612.Reset();
            psg.Reset();
            memory.Reset();
            
            // Clear frame buffer
            Array.Clear(frameBuffer, 0, frameBuffer.Length);
        }
        
        /// <summary>
        /// Run one frame of emulation.
        /// </summary>
        public void RunFrame()
        {
            if (!isInitialized || !romLoaded)
                return;
            
            int cyclesRemaining = CYCLES_PER_FRAME;
            
            // Run CPU for one frame
            while (cyclesRemaining > 0)
            {
                // Execute one CPU instruction
                int cycles = cpu.ExecuteInstruction();
                cyclesRemaining -= cycles;
                
                // Run Z80 (runs at different clock speed)
                int z80Cycles = (int)(cycles * 0.467f); // Z80 clock ratio
                z80.ExecuteCycles(z80Cycles);
                
                // Update VDP
                vdp.RunCycles(cycles);
                
                // Update sound chips
                ym2612.RunCycles(cycles);
                psg.RunCycles(cycles);
            }
            
            // Get rendered frame from VDP
            vdp.GetFrameBuffer(frameBuffer);
        }
        
        /// <summary>
        /// Set the input state.
        /// </summary>
        public void SetInput(GenesisInput input)
        {
            inputState = input;
            
            // Convert to controller byte format
            byte controller1 = 0xFF; // Active low
            
            if (input.Up) controller1 &= ~0x01;
            if (input.Down) controller1 &= ~0x02;
            if (input.Left) controller1 &= ~0x04;
            if (input.Right) controller1 &= ~0x08;
            if (input.ButtonB) controller1 &= ~0x10;
            if (input.ButtonC) controller1 &= ~0x20;
            if (input.ButtonA) controller1 &= ~0x40;
            if (input.Start) controller1 &= ~0x80;
            
            memory.SetControllerData(controller1, 0xFF); // Controller 1 and 2
        }
        
        /// <summary>
        /// Get the current frame buffer.
        /// </summary>
        public Color32[] GetFrameBuffer()
        {
            return frameBuffer;
        }
        
        /// <summary>
        /// Get audio samples for the current frame.
        /// </summary>
        public void GetAudioSamples(float[] buffer)
        {
            if (buffer == null)
                return;
            
            // Mix YM2612 and PSG audio
            ym2612.GetAudioSamples(buffer);
            
            float[] psgBuffer = new float[buffer.Length];
            psg.GetAudioSamples(psgBuffer);
            
            // Mix the two sound sources
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (buffer[i] + psgBuffer[i]) * 0.5f;
            }
        }
        
        public void Dispose()
        {
            if (cpu != null) cpu.Dispose();
            if (z80 != null) z80.Dispose();
            if (vdp != null) vdp.Dispose();
            if (ym2612 != null) ym2612.Dispose();
            if (psg != null) psg.Dispose();
            
            isInitialized = false;
            romLoaded = false;
        }
    }
}
