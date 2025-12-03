using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

namespace MDTracer.Unity
{
    /// <summary>
    /// Main Unity component for the Genesis/MegaDrive emulator.
    /// Attach this to a GameObject to run Genesis ROMs in Unity.
    /// </summary>
    [AddComponentMenu("MDTracer/Genesis Emulator")]
    public class GenesisEmulator : MonoBehaviour
    {
        [Header("ROM Settings")]
        [Tooltip("Path to the ROM file (relative to StreamingAssets or absolute path)")]
        public string romPath = "";
        
        [Tooltip("Load ROM automatically on Start")]
        public bool autoLoad = false;
        
        [Header("Display Settings")]
        [Tooltip("Target texture to render the emulator output")]
        public RenderTexture targetTexture;
        
        [Tooltip("UI RawImage to display the emulator output")]
        public RawImage targetImage;
        
        [Tooltip("Render scale (1.0 = 320x224 native, 2.0 = 640x448)")]
        [Range(1.0f, 4.0f)]
        public float renderScale = 2.0f;
        
        [Header("Emulation Settings")]
        [Tooltip("Target frame rate (Genesis runs at ~60 FPS)")]
        public int targetFrameRate = 60;
        
        [Tooltip("Enable audio output")]
        public bool enableAudio = true;
        
        [Tooltip("Audio volume (0.0 to 1.0)")]
        [Range(0.0f, 1.0f)]
        public float audioVolume = 1.0f;
        
        [Header("Debug")]
        [Tooltip("Show debug information")]
        public bool showDebugInfo = false;
        
        // Emulator core
        private GenesisCore emulatorCore;
        private Texture2D frameBuffer;
        private bool isRunning = false;
        private bool isPaused = false;
        
        // Constants
        private const int SCREEN_WIDTH = 320;
        private const int SCREEN_HEIGHT = 224;
        
        // ROM size validation (Genesis ROMs are typically 512KB to 4MB)
        private const long MAX_ROM_SIZE = 8 * 1024 * 1024; // 8MB max
        private const long MIN_ROM_SIZE = 512; // 512 bytes min (header size)
        
        void Start()
        {
            InitializeEmulator();
            
            if (autoLoad && !string.IsNullOrEmpty(romPath))
            {
                LoadROM(romPath);
            }
        }
        
        void InitializeEmulator()
        {
            // Create frame buffer texture
            int width = Mathf.RoundToInt(SCREEN_WIDTH * renderScale);
            int height = Mathf.RoundToInt(SCREEN_HEIGHT * renderScale);
            
            frameBuffer = new Texture2D(SCREEN_WIDTH, SCREEN_HEIGHT, TextureFormat.RGB24, false);
            frameBuffer.filterMode = FilterMode.Point; // Pixel-perfect scaling
            
            // Setup render texture if provided
            if (targetTexture == null && targetImage != null)
            {
                targetTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                targetTexture.filterMode = FilterMode.Point;
                targetImage.texture = targetTexture;
            }
            
            // Initialize emulator core
            emulatorCore = new GenesisCore();
            emulatorCore.Initialize();
            
            if (showDebugInfo)
            {
                Debug.Log("Genesis Emulator initialized");
            }
        }
        
        /// <summary>
        /// Load a ROM file into the emulator.
        /// </summary>
        /// <param name="path">Path to ROM file (relative to StreamingAssets or absolute)</param>
        public bool LoadROM(string path)
        {
            try
            {
                string fullPath = path;
                
                // Check if it's a relative path to StreamingAssets
                if (!Path.IsPathRooted(path))
                {
                    fullPath = Path.Combine(Application.streamingAssetsPath, path);
                }
                
                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"ROM file not found: {fullPath}");
                    return false;
                }
                
                // Validate file size
                FileInfo fileInfo = new FileInfo(fullPath);
                
                if (fileInfo.Length > MAX_ROM_SIZE)
                {
                    Debug.LogError($"ROM file too large: {fileInfo.Length} bytes (max: {MAX_ROM_SIZE})");
                    return false;
                }
                
                if (fileInfo.Length < MIN_ROM_SIZE)
                {
                    Debug.LogError($"ROM file too small: {fileInfo.Length} bytes (min: {MIN_ROM_SIZE})");
                    return false;
                }
                
                byte[] romData = File.ReadAllBytes(fullPath);
                
                if (emulatorCore.LoadROM(romData))
                {
                    isRunning = true;
                    isPaused = false;
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"ROM loaded successfully: {Path.GetFileName(fullPath)}");
                        Debug.Log($"ROM Size: {romData.Length} bytes");
                    }
                    
                    return true;
                }
                else
                {
                    Debug.LogError("Failed to load ROM");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading ROM: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Pause the emulator.
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }
        
        /// <summary>
        /// Resume the emulator.
        /// </summary>
        public void Resume()
        {
            isPaused = false;
        }
        
        /// <summary>
        /// Reset the emulator.
        /// </summary>
        public void Reset()
        {
            if (emulatorCore != null)
            {
                emulatorCore.Reset();
            }
        }
        
        /// <summary>
        /// Stop the emulator and unload the ROM.
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            isPaused = false;
        }
        
        void Update()
        {
            if (!isRunning || isPaused || emulatorCore == null)
                return;
            
            // Update input state
            UpdateInput();
            
            // Run emulator frame
            emulatorCore.RunFrame();
            
            // Update display
            UpdateDisplay();
        }
        
        void UpdateInput()
        {
            // Map Unity input to Genesis controller
            // Genesis has: Up, Down, Left, Right, A, B, C, Start
            
            GenesisInput input = new GenesisInput();
            
            // D-Pad
            input.Up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            input.Down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            input.Left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            input.Right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
            
            // Buttons
            input.ButtonA = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.J);
            input.ButtonB = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.K);
            input.ButtonC = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.L);
            input.Start = Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
            
            emulatorCore.SetInput(input);
        }
        
        void UpdateDisplay()
        {
            // Get frame buffer from emulator
            Color32[] pixels = emulatorCore.GetFrameBuffer();
            
            if (pixels != null && pixels.Length == SCREEN_WIDTH * SCREEN_HEIGHT)
            {
                frameBuffer.SetPixels32(pixels);
                frameBuffer.Apply();
                
                // Blit to render texture if available
                if (targetTexture != null)
                {
                    Graphics.Blit(frameBuffer, targetTexture);
                }
                
                // Update UI image if available
                if (targetImage != null && targetImage.texture == null)
                {
                    targetImage.texture = frameBuffer;
                }
            }
        }
        
        void OnDestroy()
        {
            if (emulatorCore != null)
            {
                emulatorCore.Dispose();
            }
            
            if (frameBuffer != null)
            {
                Destroy(frameBuffer);
            }
        }
        
        void OnGUI()
        {
            if (showDebugInfo && isRunning)
            {
                GUILayout.BeginArea(new Rect(10, 10, 300, 200));
                GUILayout.Label($"Genesis Emulator - Running");
                GUILayout.Label($"FPS: {1.0f / Time.deltaTime:F1}");
                GUILayout.Label($"Paused: {isPaused}");
                GUILayout.Label($"Controls:");
                GUILayout.Label("  Arrow Keys/WASD - D-Pad");
                GUILayout.Label("  Z/J - Button A");
                GUILayout.Label("  X/K - Button B");
                GUILayout.Label("  C/L - Button C");
                GUILayout.Label("  Enter - Start");
                GUILayout.EndArea();
            }
        }
    }
    
    /// <summary>
    /// Input state for Genesis controller
    /// </summary>
    public struct GenesisInput
    {
        public bool Up;
        public bool Down;
        public bool Left;
        public bool Right;
        public bool ButtonA;
        public bool ButtonB;
        public bool ButtonC;
        public bool Start;
    }
}
