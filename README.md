# Pong Unity

A complete beginner-friendly 2D Pong project for Unity.

## What is included
- A self-contained Unity project folder.
- Runtime scripts for a real Pong loop: mode select, paddles, ball movement, bounces, scoring, win state, rematches, and reset behavior.
- Two playable modes:
  - Player vs Computer
  - Player vs Player on the same keyboard
- An editor setup script that creates the playable scene automatically when the project opens in Unity.
- A runtime bootstrap, so even pressing Play in Unity's default empty scene shows the Pong menu.
- No imported art or paid assets. Everything is generated from simple Unity objects.

## How to open it
1. Unzip the project if it was sent as a zip.
2. Open `CHECK_THIS_FIRST.txt` and confirm the listed files exist.
3. Optional on macOS: double-click `verify-pong-project.command`.
4. Install Unity Hub and a Unity 6 editor version.
5. In Unity Hub, choose **Add project from disk**.
6. Select this folder: `PongUnity`.
7. Open the project.
8. Press **Play**.

If the scene is not created automatically, use the Unity top menu:

`Pong > Create Or Refresh Complete Scene`

If you still only see Unity's default sky/ground view after pressing Play, check the Project panel. You should see:

- `Assets/Scripts`
- `Assets/Editor`

If those folders are missing, Unity opened the wrong or incomplete folder. Close Unity, delete that copied `PongUnity` folder from the other laptop, then send/open the updated full `PongUnity` folder again.

## Controls
- Menu: `1` for Player vs Computer, `2` for Player vs Player
- Player 1: `W` / `S`
- Player 2: `Up Arrow` / `Down Arrow`
- Rematch/restart: `R`
- After game over: `Space` or `Enter` for rematch
- Return to menu: `M` or `Escape`
- Quit built player: `Escape`

## Game rules
- First player to 7 points wins.
- In Player vs Computer, the right paddle is AI-controlled.
- In Player vs Player, both paddles are controlled from the keyboard.
- The ball serves from center after each point.

## Notes
- The local machine did not have Unity installed when this was created, so the files are prepared for Unity but could not be opened in-editor here.
- Once Unity is installed, the editor script can also be run from the command line with Unity's `-projectPath` and `-executeMethod` arguments.

## Troubleshooting
If Unity opens in Safe Mode with errors saying `Rigidbody2D`, `Collider2D`, `Collision2D`, or `PhysicsMaterial2D` cannot be found, the project was opened before the built-in Physics 2D module was enabled.

1. Close Unity.
2. Reopen this project from Unity Hub.
3. If Unity asks whether to enter Safe Mode, choose to ignore/exit Safe Mode after packages finish resolving.
4. If errors remain, open **Window > Package Manager**, switch the package filter to **Built-in**, and make sure **Physics 2D** is enabled.
