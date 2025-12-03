# Installation Guide

## Quick Install via Unity Package Manager (Recommended)

1. **Open Unity** (2020.3 or newer)

2. **Open Package Manager**
   - Go to `Window > Package Manager`

3. **Add Package from Git**
   - Click the `+` button in the top-left corner
   - Select `Add package from git URL...`

4. **Enter Git URL**
   ```
   https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator.git?path=/UnityPackage
   ```

5. **Wait for Installation**
   - Unity will download and install the package
   - This may take a few moments

6. **Verify Installation**
   - The package should appear in the Package Manager as "MDTracer Genesis Emulator"
   - You should see it under "Custom" or "Packages" in your project

## Manual Installation

If you prefer to install manually or the Git URL method doesn't work:

1. **Clone or Download Repository**
   ```bash
   git clone https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator.git
   ```

2. **Copy Package to Your Project**
   - Copy the `UnityPackage` folder
   - Paste it into your Unity project's `Packages` folder
   - If the `Packages` folder doesn't exist, create it in your project root

3. **Unity Will Auto-Import**
   - Unity will automatically detect and import the package
   - You may need to wait a moment for Unity to recompile

## Post-Installation Setup

### 1. Import Sample Scene

1. Open Package Manager
2. Find "MDTracer Genesis Emulator" in the list
3. Expand the "Samples" section
4. Click "Import" next to "Basic Emulator"
5. The sample will be imported to `Assets/Samples/MDTracer Genesis Emulator/1.0.0/BasicEmulator/`

### 2. Prepare ROM Files

1. **Create StreamingAssets Folder**
   - Right-click in Project window
   - `Create > Folder`
   - Name it `StreamingAssets`

2. **Add Your ROM Files**
   - Place your legally obtained Genesis/MegaDrive ROM files in `Assets/StreamingAssets/`
   - Supported formats: `.bin`, `.gen`, `.md` (uncompressed)

3. **⚠️ Legal Notice**
   - Only use ROM files you legally own
   - Do not distribute copyrighted ROM files
   - This package does not include any game ROMs

### 3. Create Your First Emulator Scene

1. **Create New Scene**
   - `File > New Scene`

2. **Add Emulator GameObject**
   - `GameObject > Create Empty`
   - Rename to "GenesisEmulator"

3. **Add Component**
   - Select the GameObject
   - `Add Component > MDTracer > Genesis Emulator`

4. **Create UI Canvas** (for display)
   - `GameObject > UI > Canvas`
   - Set Canvas Scaler to "Scale with Screen Size"
   - Reference Resolution: 1920x1080

5. **Add Raw Image** (for emulator output)
   - Right-click Canvas
   - `UI > Raw Image`
   - Rename to "EmulatorDisplay"
   - Set anchors to stretch (full screen or desired size)

6. **Configure Emulator Component**
   - Select GenesisEmulator GameObject
   - In Inspector, find "Genesis Emulator" component:
     - Set "Rom Path" to your ROM filename (e.g., "sonic.bin")
     - Drag EmulatorDisplay to "Target Image" field
     - Enable "Auto Load"
     - Set "Render Scale" to 2.0 (or adjust to preference)

7. **Press Play!**
   - Your ROM should load automatically
   - Use arrow keys / WASD to play

## Default Controls

| Input | Genesis Button |
|-------|---------------|
| Arrow Keys / WASD | D-Pad |
| Z / J | Button A |
| X / K | Button B |
| C / L | Button C |
| Enter | Start |

## Troubleshooting

### Package Not Appearing

**Problem:** Package doesn't show up in Package Manager

**Solutions:**
- Make sure you're using Unity 2020.3 or newer
- Check that the Git URL is correct
- Try the manual installation method
- Check Unity's console for error messages

### ROM Not Loading

**Problem:** ROM file not found error

**Solutions:**
- Verify ROM file is in `Assets/StreamingAssets/`
- Check that the filename in "Rom Path" exactly matches your file
- Make sure ROM file is not corrupted
- Try using an absolute path instead of relative path

### Black Screen

**Problem:** Emulator shows black screen

**Solutions:**
- Check that Target Image is properly assigned
- Verify Raw Image is visible in Game view
- Check that Auto Load is enabled
- Make sure ROM is valid Genesis/MegaDrive format

**Note:** Current version shows a test pattern since full emulation cores are not yet integrated. See [ImplementationGuide.md](Documentation~/ImplementationGuide.md) for integration status.

### Performance Issues

**Problem:** Low frame rate or stuttering

**Solutions:**
- Reduce Render Scale (try 1.0)
- Check Unity Profiler for bottlenecks
- Make sure VSync is disabled in Project Settings
- Try standalone build instead of Editor playback

### Build Errors

**Problem:** Compilation errors when importing

**Solutions:**
- Make sure you have .NET Standard 2.0 or higher
- Check that "Allow Unsafe Code" is enabled in Player Settings
- Update to latest Unity LTS version
- Check error messages for missing dependencies

## Updating the Package

### Via Package Manager (Git URL)

1. Open Package Manager
2. Select "MDTracer Genesis Emulator"
3. Click the update button if available
4. Or remove and re-add the package with the Git URL

### Manual Update

1. Delete the old `Packages/UnityPackage` folder
2. Copy the new version to `Packages/`
3. Unity will reimport automatically

## Uninstalling

### Via Package Manager

1. Open Package Manager
2. Find "MDTracer Genesis Emulator"
3. Click "Remove"

### Manual Uninstall

1. Delete `Packages/UnityPackage` folder
2. Unity will automatically remove references

## Getting Help

- **Documentation:** See [README.md](README.md) for usage guide
- **Implementation:** See [ImplementationGuide.md](Documentation~/ImplementationGuide.md)
- **GitHub Issues:** https://github.com/masterrey/MDTracer-Genesis-megadrive-Emulator/issues
- **Original Project:** https://www.jppass.jp/mdtracer

## System Requirements

- **Unity Version:** 2020.3 or newer
- **Scripting Backend:** Mono or IL2CPP
- **.NET Version:** .NET Standard 2.0 or .NET 4.x
- **Platforms:** Tested on Windows, should work on macOS and Linux
- **RAM:** Minimal (emulator is lightweight)
- **Storage:** Package is ~1MB, ROM files vary (typically 512KB-4MB)

## Next Steps

After installation:
1. Read [README.md](README.md) for usage examples
2. Import the sample scene to see it in action
3. Check [UNITY_INTEGRATION.md](../UNITY_INTEGRATION.md) for architecture details
4. Explore [ImplementationGuide.md](Documentation~/ImplementationGuide.md) if you want to contribute

Enjoy emulating Genesis/MegaDrive games in Unity! 🎮
