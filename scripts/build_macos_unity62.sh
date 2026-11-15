#!/bin/bash
set -euo pipefail
PROJECT_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_EDITOR="${UNITY_EDITOR:-$PROJECT_ROOT/tools/unity-6000.2.8f1/Unity.app/Contents/MacOS/Unity}"
exec "$UNITY_EDITOR" -batchmode -nographics -quit -accept-apiupdate \
  -projectPath "$PROJECT_ROOT/recovery-unity62/ExportedProject" \
  -buildTarget StandaloneOSX -executeMethod MacPortBuild.Build \
  -refreshImportMode outofprocessperqueue -desiredWorkerCount 4 \
  -logFile "$PROJECT_ROOT/logs/unity62-build.log"
