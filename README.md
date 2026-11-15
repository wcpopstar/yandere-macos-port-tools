# Experimental macOS porting utilities

Unofficial research utilities developed while investigating a native macOS port
of Yandere Simulator. This repository contains only separately written build,
runtime compatibility and shader-generation helpers. It contains no game build,
recovered game source, assets, original DLLs, editor binaries, logs or saves.
It is not an official release and is not endorsed by the game developer.

## Scope

- MacPortBuild.cs: Unity editor build entry point (Mono, Metal, macOS).
- MacPortRuntime.cs: writable user-data directory and temporary bypass for
  screen-space effects whose original shaders have not been restored.
- repair_ngui_shaders.py: generates basic UI shader replacements in a local
  recovered project, preserving backups.
- build_macos_unity62.sh: invokes a locally installed matching Unity editor.

These utilities require an already prepared local Unity project. The local
game project, other compatibility edits, dependencies and matching editor are
excluded. The shader script expects
recovery-unity62/ExportedProject and the build script expects Unity 6000.2.8f1
under tools/unity-6000.2.8f1 (or UNITY_EDITOR set to its executable).
Only use these tools with material you are entitled to process. No rights to
redistribute the game or third-party assets are granted by this repository.
No blanket open-source license has been assigned to the original game.

## macOS preview status

The local port targets macOS 12 or later and includes arm64 and x86_64 binaries.
The release build reached the main menu on Apple M4 with Metal. Earlier
development builds also loaded home and school in both 2026 and 1989 modes.
Intel gameplay and full campaigns are pending verification. Some shaders are
approximations and the optional Windows YouTube integration is disabled.

The official game download is available from [Yandere Simulator](https://yanderesimulator.com/download/).
There is currently no installer here that turns that Windows download into
the native macOS preview. The local preview bundles original game data.
