#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

UNITY_BIN="$(find /Applications/Unity/Hub/Editor -path '*/Unity.app/Contents/MacOS/Unity' -type f 2>/dev/null | sort -Vr | head -n 1 || true)"

if [[ -z "${UNITY_BIN}" ]]; then
  echo "Unity was not found in /Applications/Unity/Hub/Editor."
  echo "Install Unity Hub + a Unity 6 editor, then open this folder from Unity Hub:"
  echo "${PROJECT_DIR}"
  exit 1
fi

"${UNITY_BIN}" \
  -projectPath "${PROJECT_DIR}" \
  -executeMethod PongSceneBuilder.CreateOrRefreshScene
