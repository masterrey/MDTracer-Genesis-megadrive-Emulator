# Changelog

All notable changes to the MDTracer Genesis Emulator Unity package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-12-03

### Added
- Initial Unity package structure
- `GenesisEmulator` MonoBehaviour component for easy integration
- Package Manager support with package.json
- Assembly definition files for proper code organization
- Core emulator architecture with stub implementations:
  - M68K CPU processor stub
  - Z80 CPU processor stub
  - VDP graphics chip stub
  - YM2612 FM sound chip stub
  - SN76489 PSG sound chip stub
  - Memory bus system
  - Cartridge ROM loading and header parsing
- Unity-compatible input handling (keyboard)
- Texture-based rendering output
- Custom Editor inspector for GenesisEmulator component
- Sample scene and UI controller
- Comprehensive documentation:
  - README with quick start guide
  - Integration guide explaining architecture
  - Sample code and examples

### In Progress
- Full CPU emulation integration from original MDTracer
- Complete VDP rendering implementation
- Sound chip emulation
- Audio output via Unity AudioSource

### Known Limitations
- Emulation core uses stub implementations (will not run actual ROMs yet)
- Audio output not yet implemented
- No save state support
- No configuration for controller remapping (coming soon)
- Mobile platform optimization needed

## Future Versions

### Planned for 1.1.0
- Complete M68K CPU integration
- Basic VDP rendering
- ROM execution support

### Planned for 1.2.0
- Z80 CPU integration
- Sound chip integration
- Audio output

### Planned for 1.3.0
- Save states
- Configurable input mapping
- Performance optimizations

### Planned for 2.0.0
- Mobile platform support
- Advanced debugging features
- Network multiplayer support
