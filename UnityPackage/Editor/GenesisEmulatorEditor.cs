using UnityEngine;
using UnityEditor;
using MDTracer.Unity;

namespace MDTracer.Unity.Editor
{
    /// <summary>
    /// Custom editor for the GenesisEmulator component.
    /// Provides a better inspector experience with helpful buttons and information.
    /// </summary>
    [CustomEditor(typeof(GenesisEmulator))]
    public class GenesisEmulatorEditor : UnityEditor.Editor
    {
        private SerializedProperty romPathProp;
        private SerializedProperty autoLoadProp;
        private SerializedProperty targetTextureProp;
        private SerializedProperty targetImageProp;
        private SerializedProperty renderScaleProp;
        private SerializedProperty targetFrameRateProp;
        private SerializedProperty enableAudioProp;
        private SerializedProperty audioVolumeProp;
        private SerializedProperty showDebugInfoProp;
        
        // Cache for path validation to avoid repeated file system checks
        private string cachedRomPath = "";
        private bool cachedPathExists = false;
        
        private void OnEnable()
        {
            romPathProp = serializedObject.FindProperty("romPath");
            autoLoadProp = serializedObject.FindProperty("autoLoad");
            targetTextureProp = serializedObject.FindProperty("targetTexture");
            targetImageProp = serializedObject.FindProperty("targetImage");
            renderScaleProp = serializedObject.FindProperty("renderScale");
            targetFrameRateProp = serializedObject.FindProperty("targetFrameRate");
            enableAudioProp = serializedObject.FindProperty("enableAudio");
            audioVolumeProp = serializedObject.FindProperty("audioVolume");
            showDebugInfoProp = serializedObject.FindProperty("showDebugInfo");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GenesisEmulator emulator = (GenesisEmulator)target;
            
            // Header
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Genesis/MegaDrive Emulator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // ROM Settings
            EditorGUILayout.LabelField("ROM Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(romPathProp);
            EditorGUILayout.PropertyField(autoLoadProp);
            
            if (!string.IsNullOrEmpty(romPathProp.stringValue))
            {
                // Only recompute path if it changed (performance optimization)
                if (cachedRomPath != romPathProp.stringValue)
                {
                    cachedRomPath = romPathProp.stringValue;
                    string fullPath = System.IO.Path.IsPathRooted(cachedRomPath) 
                        ? cachedRomPath 
                        : System.IO.Path.Combine(Application.streamingAssetsPath, cachedRomPath);
                    cachedPathExists = System.IO.File.Exists(fullPath);
                }
                
                if (cachedPathExists)
                {
                    string fileName = System.IO.Path.GetFileName(cachedRomPath);
                    EditorGUILayout.HelpBox($"ROM file found: {fileName}", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("ROM file not found. Make sure the path is correct and the file exists in StreamingAssets.", MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space();
            
            // Display Settings
            EditorGUILayout.LabelField("Display Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetTextureProp);
            EditorGUILayout.PropertyField(targetImageProp);
            EditorGUILayout.PropertyField(renderScaleProp);
            
            int outputWidth = Mathf.RoundToInt(320 * renderScaleProp.floatValue);
            int outputHeight = Mathf.RoundToInt(224 * renderScaleProp.floatValue);
            EditorGUILayout.HelpBox($"Output Resolution: {outputWidth}x{outputHeight}", MessageType.None);
            
            EditorGUILayout.Space();
            
            // Emulation Settings
            EditorGUILayout.LabelField("Emulation Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetFrameRateProp);
            EditorGUILayout.PropertyField(enableAudioProp);
            
            if (enableAudioProp.boolValue)
            {
                EditorGUILayout.PropertyField(audioVolumeProp);
            }
            
            EditorGUILayout.Space();
            
            // Debug
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showDebugInfoProp);
            
            EditorGUILayout.Space();
            
            // Runtime Controls (only in Play mode)
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Load ROM"))
                {
                    if (!string.IsNullOrEmpty(romPathProp.stringValue))
                    {
                        emulator.LoadROM(romPathProp.stringValue);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Please specify a ROM path first.", "OK");
                    }
                }
                
                if (GUILayout.Button("Pause"))
                {
                    emulator.Pause();
                }
                
                if (GUILayout.Button("Resume"))
                {
                    emulator.Resume();
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Reset"))
                {
                    emulator.Reset();
                }
                
                if (GUILayout.Button("Stop"))
                {
                    emulator.Stop();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            // Help Box
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Place your Genesis ROM files in the StreamingAssets folder. " +
                "Make sure you legally own any ROMs you use with this emulator.",
                MessageType.Info);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
