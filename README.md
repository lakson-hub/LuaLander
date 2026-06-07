# LuaLander

LuaLander is a 2D Unity game about piloting a lunar lander, managing fuel, dodging wind, collecting pickups, and touching down safely on the correct pad.

## Languages & Tools

<p align="left">
  <img src="https://img.shields.io/badge/Unity-6000.3.15f1-000000?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 6000.3.15f1" height="36" />
  <img src="https://raw.githubusercontent.com/dotnet/vscode-csharp/main/images/csharpIcon.png" alt="C# logo" height="36" />
  <img src="https://upload.wikimedia.org/wikipedia/commons/6/6e/JetBrains_Rider_Icon.svg" alt="JetBrains Rider logo" height="36" />
</p>

## Features

- 2D lander physics with thrust, rotation, and fuel management
- Multiple handcrafted levels
- Landing pads with score multipliers
- Fuel and coin pickups
- Wind zones that affect flight
- Main menu, pause flow, gameplay HUD, and game over screen
- Cinemachine camera behavior and URP rendering

## Controls

- `W` / `Up Arrow` - Thrust up
- `A` / `Left Arrow` - Rotate or move left
- `D` / `Right Arrow` - Rotate or move right
- `Left Stick` on gamepad - Move / thrust
- `Esc` - Pause or unpause the game
- `Start` on gamepad - Pause or unpause the game

## How To Play

1. Launch the game from the main menu.
2. Use thrust carefully to stay in control.
3. Land on the correct landing pad as gently and upright as possible.
4. Collect coins and fuel pickups to improve your score and survivability.
5. Watch out for wind, steep angles, and high landing speed.
6. Clear all levels before running out of lives.

## Getting Started

### Requirements

- Unity 6000.3.15f1
- JetBrains Rider or another C# editor

### Open The Project

1. Clone the repository.
2. Open the folder in Unity Hub.
3. Select Unity version `6000.3.15f1`.
4. Open the project and let Unity import the assets.
5. Open `Assets/Scenes/MainMenuScene.unity` to start from the menu.

## Project Structure

- `Assets/Scenes` - Main menu, gameplay, game over, and sample scenes
- `Assets/Scripts` - Core game logic, UI, input, and scene loading
- `Assets/Prefabs` - Landing pads, pickups, particles, music manager, and wind prefabs
- `Assets/Textures` - Sprite and UI art
- `Assets/Sounds` - Music and sound effects
- `ProjectSettings` - Unity project configuration

## Credits

- Tutorial / inspiration source: [YouTube video](https://www.youtube.com/watch?v=nGKd4yTP3M8&list=PLzDRvYVwl53uAyV0SjL_3d_IoRDiybAdN&index=1)
- TextMesh Pro font assets and other Unity package content are included in the project
- Try my game for free: [Unity Play](https://play.unity.com/en/games/38ccc7a9-ed57-4e5d-bb1a-0bb3981522f6/lua-lander), [Itch.io](https://pr0fit.itch.io/lua-lander)

## Notes

- This project uses the Unity Input System package.
- Game scenes are loaded through `SceneLoader`, and game state is managed by `GameManager`.
