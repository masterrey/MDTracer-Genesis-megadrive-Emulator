using UnityEngine;
using UnityEngine.UI;
using MDTracer.Unity;

namespace MDTracer.Unity.Samples
{
    /// <summary>
    /// Example UI controller for the Genesis Emulator.
    /// Demonstrates how to control the emulator from script.
    /// </summary>
    public class EmulatorUI : MonoBehaviour
    {
        [Header("References")]
        public GenesisEmulator emulator;
        public Button pauseButton;
        public Button resetButton;
        public Button stopButton;
        public Text statusText;
        
        [Header("ROM Selection")]
        public InputField romPathInput;
        public Button loadButton;
        
        private bool isPaused = false;
        
        void Start()
        {
            // Setup button listeners
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClick);
            }
            
            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetClick);
            }
            
            if (stopButton != null)
            {
                stopButton.onClick.AddListener(OnStopClick);
            }
            
            if (loadButton != null)
            {
                loadButton.onClick.AddListener(OnLoadClick);
            }
            
            UpdateStatus("Ready");
        }
        
        void Update()
        {
            // Keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.P))
            {
                OnPauseClick();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnResetClick();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnStopClick();
            }
        }
        
        void OnPauseClick()
        {
            if (emulator == null)
                return;
            
            isPaused = !isPaused;
            
            if (isPaused)
            {
                emulator.Pause();
                UpdateStatus("Paused");
                
                if (pauseButton != null)
                {
                    var text = pauseButton.GetComponentInChildren<Text>();
                    if (text != null) text.text = "Resume";
                }
            }
            else
            {
                emulator.Resume();
                UpdateStatus("Running");
                
                if (pauseButton != null)
                {
                    var text = pauseButton.GetComponentInChildren<Text>();
                    if (text != null) text.text = "Pause";
                }
            }
        }
        
        void OnResetClick()
        {
            if (emulator == null)
                return;
            
            emulator.Reset();
            isPaused = false;
            UpdateStatus("Reset");
            
            if (pauseButton != null)
            {
                var text = pauseButton.GetComponentInChildren<Text>();
                if (text != null) text.text = "Pause";
            }
        }
        
        void OnStopClick()
        {
            if (emulator == null)
                return;
            
            emulator.Stop();
            isPaused = false;
            UpdateStatus("Stopped");
            
            if (pauseButton != null)
            {
                var text = pauseButton.GetComponentInChildren<Text>();
                if (text != null) text.text = "Pause";
            }
        }
        
        void OnLoadClick()
        {
            if (emulator == null || romPathInput == null)
                return;
            
            string romPath = romPathInput.text;
            
            if (string.IsNullOrEmpty(romPath))
            {
                UpdateStatus("Error: No ROM path specified");
                return;
            }
            
            UpdateStatus("Loading ROM...");
            
            if (emulator.LoadROM(romPath))
            {
                UpdateStatus("ROM loaded successfully");
                isPaused = false;
            }
            else
            {
                UpdateStatus("Error: Failed to load ROM");
            }
        }
        
        void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            
#if UNITY_EDITOR
            Debug.Log($"Emulator: {message}");
#endif
        }
    }
}
