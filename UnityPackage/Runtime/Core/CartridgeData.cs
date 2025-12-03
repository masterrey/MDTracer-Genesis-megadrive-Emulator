using System;
using System.Text;
using UnityEngine;

namespace MDTracer.Unity
{
    /// <summary>
    /// Cartridge data and ROM management.
    /// Handles loading and parsing Genesis/MegaDrive ROM files.
    /// </summary>
    public class CartridgeData
    {
        private byte[] romData;
        private int romSize;
        
        // Cartridge header information
        public string SystemType { get; private set; }
        public string Copyright { get; private set; }
        public string GameTitle { get; private set; }
        public string SerialNumber { get; private set; }
        public string Region { get; private set; }
        public uint RomStart { get; private set; }
        public uint RomEnd { get; private set; }
        
        public CartridgeData()
        {
            romData = null;
            romSize = 0;
        }
        
        /// <summary>
        /// Load a ROM file into the cartridge.
        /// </summary>
        public bool LoadROM(byte[] data)
        {
            if (data == null || data.Length < 512)
            {
                Debug.LogError("Invalid ROM data");
                return false;
            }
            
            romData = data;
            romSize = data.Length;
            
            // Parse header information
            ParseHeader();
            
            return true;
        }
        
        private void ParseHeader()
        {
            try
            {
                // Genesis/MegaDrive ROM header starts at 0x100
                
                // System type (0x100-0x10F)
                SystemType = ReadString(0x100, 16).Trim();
                
                // Copyright (0x110-0x11F)
                Copyright = ReadString(0x110, 16).Trim();
                
                // Domestic name (0x120-0x14F)
                GameTitle = ReadString(0x120, 48).Trim();
                
                // Serial number (0x180-0x18D)
                SerialNumber = ReadString(0x180, 14).Trim();
                
                // Region (0x1F0-0x1FF)
                Region = ReadString(0x1F0, 16).Trim();
                
                // ROM start/end addresses
                RomStart = ReadLong(0x1A0);
                RomEnd = ReadLong(0x1A4);
                
                Debug.Log($"Cartridge loaded: {GameTitle}");
                Debug.Log($"  System: {SystemType}");
                Debug.Log($"  Copyright: {Copyright}");
                Debug.Log($"  Serial: {SerialNumber}");
                Debug.Log($"  Region: {Region}");
                Debug.Log($"  ROM: 0x{RomStart:X8} - 0x{RomEnd:X8}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error parsing ROM header: {ex.Message}");
                
                // Set defaults if header parsing fails
                SystemType = "Unknown";
                Copyright = "Unknown";
                GameTitle = "Unknown ROM";
                SerialNumber = "Unknown";
                Region = "Unknown";
            }
        }
        
        private string ReadString(int offset, int length)
        {
            if (offset + length > romSize)
                return "";
            
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                byte b = romData[offset + i];
                if (b >= 0x20 && b < 0x7F) // Printable ASCII
                {
                    sb.Append((char)b);
                }
                else if (b == 0x00)
                {
                    break; // Null terminator
                }
            }
            
            return sb.ToString();
        }
        
        private uint ReadLong(int offset)
        {
            if (offset + 4 > romSize)
                return 0;
            
            return (uint)((romData[offset] << 24) |
                         (romData[offset + 1] << 16) |
                         (romData[offset + 2] << 8) |
                         romData[offset + 3]);
        }
        
        /// <summary>
        /// Read a byte from the ROM.
        /// </summary>
        public byte ReadByte(uint address)
        {
            if (romData == null || address >= romSize)
                return 0xFF;
            
            return romData[address];
        }
        
        /// <summary>
        /// Read a word (16-bit) from the ROM.
        /// </summary>
        public ushort ReadWord(uint address)
        {
            byte hi = ReadByte(address);
            byte lo = ReadByte(address + 1);
            return (ushort)((hi << 8) | lo);
        }
        
        /// <summary>
        /// Read a long (32-bit) from the ROM.
        /// </summary>
        public uint ReadLong(uint address)
        {
            ushort hi = ReadWord(address);
            ushort lo = ReadWord(address + 2);
            return (uint)((hi << 16) | lo);
        }
        
        /// <summary>
        /// Get the ROM size in bytes.
        /// </summary>
        public int GetSize()
        {
            return romSize;
        }
        
        /// <summary>
        /// Check if a ROM is loaded.
        /// </summary>
        public bool IsLoaded()
        {
            return romData != null && romSize > 0;
        }
    }
}
