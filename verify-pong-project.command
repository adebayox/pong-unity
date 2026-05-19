#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

required_files=(
  "Assets/Scripts/PongGame.cs"
  "Assets/Scripts/PongBootstrap.cs"
  "Assets/Scripts/PongBall.cs"
  "Assets/Scripts/PongPaddle.cs"
  "Assets/Scripts/PongGoal.cs"
  "Assets/Scripts/PongVelocity.cs"
  "Assets/Editor/PongSceneBuilder.cs"
  "Packages/manifest.json"
)

missing=0
for file in "${required_files[@]}"; do
  if [[ -f "${PROJECT_DIR}/${file}" ]]; then
    echo "OK: ${file}"
  else
    echo "MISSING: ${file}"
    missing=1
  fi
done

if [[ "${missing}" -ne 0 ]]; then
  echo
  echo "This PongUnity folder is incomplete. Delete it and unzip/open the full project again."
  exit 1
fi

echo
echo "PongUnity folder looks complete. Open this folder from Unity Hub, then press Play."
