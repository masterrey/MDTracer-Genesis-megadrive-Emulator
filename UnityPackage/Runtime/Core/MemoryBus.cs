using System;

namespace MDTracer.Unity
{
    /// <summary>
    /// Memory bus that connects all components (CPU, VDP, cartridge, etc.)
    /// Handles memory mapping and I/O for the Genesis/MegaDrive system.
    /// </summary>
    public class MemoryBus
    {
        private CartridgeData cartridge;
        private VDPChip vdp;
        private Z80Processor z80;
        private YM2612SoundChip ym2612;
        private SN76489SoundChip psg;
        
        // System RAM
        private byte[] workRam;  // 64KB work RAM at 0xFF0000-0xFFFFFF
        private byte[] z80Ram;   // 8KB Z80 RAM at 0xA00000-0xA01FFF
        
        // I/O
        private byte controller1Data;
        private byte controller2Data;
        private bool z80BusRequest;
        private bool z80Reset;
        
        public MemoryBus()
        {
            workRam = new byte[65536];   // 64KB
            z80Ram = new byte[8192];     // 8KB
            controller1Data = 0xFF;
            controller2Data = 0xFF;
        }
        
        public void SetCartridge(CartridgeData cart)
        {
            cartridge = cart;
        }
        
        public void SetVDP(VDPChip vdpChip)
        {
            vdp = vdpChip;
        }
        
        public void SetZ80(Z80Processor z80Cpu)
        {
            z80 = z80Cpu;
        }
        
        public void SetSoundChips(YM2612SoundChip ym, SN76489SoundChip psgChip)
        {
            ym2612 = ym;
            psg = psgChip;
        }
        
        public void Reset()
        {
            Array.Clear(workRam, 0, workRam.Length);
            Array.Clear(z80Ram, 0, z80Ram.Length);
            
            controller1Data = 0xFF;
            controller2Data = 0xFF;
            z80BusRequest = false;
            z80Reset = false;
        }
        
        public void SetControllerData(byte controller1, byte controller2)
        {
            controller1Data = controller1;
            controller2Data = controller2;
        }
        
        // Memory read operations
        public byte ReadByte(uint address)
        {
            // ROM: 0x000000 - 0x3FFFFF (4MB)
            if (address < 0x400000)
            {
                return cartridge != null ? cartridge.ReadByte(address) : (byte)0xFF;
            }
            // Z80 address space: 0xA00000 - 0xA0FFFF
            else if (address >= 0xA00000 && address < 0xA10000)
            {
                if (address < 0xA02000)
                {
                    return z80Ram[address & 0x1FFF];
                }
                return 0xFF;
            }
            // Z80 control: 0xA11100, 0xA11200
            else if (address == 0xA11100)
            {
                return z80BusRequest ? (byte)0x00 : (byte)0x01;
            }
            else if (address == 0xA11200)
            {
                return z80Reset ? (byte)0x00 : (byte)0x01;
            }
            // I/O: 0xA10000 - 0xA10FFF
            else if (address >= 0xA10000 && address < 0xA11000)
            {
                return ReadIO(address);
            }
            // VDP: 0xC00000 - 0xC0001F
            else if (address >= 0xC00000 && address < 0xC00020)
            {
                return ReadVDP(address);
            }
            // Work RAM: 0xFF0000 - 0xFFFFFF
            else if (address >= 0xFF0000)
            {
                return workRam[address & 0xFFFF];
            }
            
            return 0xFF;
        }
        
        public ushort ReadWord(uint address)
        {
            byte hi = ReadByte(address);
            byte lo = ReadByte(address + 1);
            return (ushort)((hi << 8) | lo);
        }
        
        public uint ReadLong(uint address)
        {
            ushort hi = ReadWord(address);
            ushort lo = ReadWord(address + 2);
            return (uint)((hi << 16) | lo);
        }
        
        // Memory write operations
        public void WriteByte(uint address, byte value)
        {
            // ROM area is read-only
            if (address < 0x400000)
            {
                return;
            }
            // Z80 address space
            else if (address >= 0xA00000 && address < 0xA10000)
            {
                if (address < 0xA02000)
                {
                    z80Ram[address & 0x1FFF] = value;
                }
            }
            // Z80 control
            else if (address == 0xA11100)
            {
                z80BusRequest = (value & 0x01) != 0;
                if (z80 != null)
                {
                    z80.SetActive(!z80BusRequest);
                }
            }
            else if (address == 0xA11200)
            {
                z80Reset = (value & 0x01) == 0;
                if (z80 != null)
                {
                    z80.SetReset(z80Reset);
                }
            }
            // I/O
            else if (address >= 0xA10000 && address < 0xA11000)
            {
                WriteIO(address, value);
            }
            // VDP
            else if (address >= 0xC00000 && address < 0xC00020)
            {
                WriteVDP(address, value);
            }
            // Work RAM
            else if (address >= 0xFF0000)
            {
                workRam[address & 0xFFFF] = value;
            }
        }
        
        public void WriteWord(uint address, ushort value)
        {
            WriteByte(address, (byte)(value >> 8));
            WriteByte(address + 1, (byte)(value & 0xFF));
        }
        
        public void WriteLong(uint address, uint value)
        {
            WriteWord(address, (ushort)(value >> 16));
            WriteWord(address + 2, (ushort)(value & 0xFFFF));
        }
        
        private byte ReadIO(uint address)
        {
            // Controller ports
            if (address == 0xA10003) // Controller 1 data
            {
                return controller1Data;
            }
            else if (address == 0xA10005) // Controller 2 data
            {
                return controller2Data;
            }
            
            return 0xFF;
        }
        
        private void WriteIO(uint address, byte value)
        {
            // I/O control registers
            // TODO: Implement full I/O port control
        }
        
        private byte ReadVDP(uint address)
        {
            if (vdp == null)
                return 0xFF;
            
            // VDP data port and control port
            // TODO: Implement full VDP port access
            return 0xFF;
        }
        
        private void WriteVDP(uint address, byte value)
        {
            if (vdp == null)
                return;
            
            // VDP data port and control port
            // TODO: Implement full VDP port access
        }
    }
}
