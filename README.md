# 2_5D_Circus
This is a 2.5D board-style game project developed in Unity as part of a semester assignment. The game demonstrates use of animated UI, player-controlled characters, turn-based logic, and basic C# scripting.

The game includes a character selection system, a dice rolling mechanic, special tiles with effects (hurt and chest), a scoring system with leaderboard, and JSON-based saving/loading.

---

## Features

- Dice rolling and tile-based movement
- Hurt, bridge and chest tiles with special gameplay effects
- Leaderboard that records player name, time, and score
- Score calculation based on time, dice rolls, hurt and chest events
- Character selection with unique prefabs
- Save/load using JSON
- Scrollable ranking panel
- Pause menu with settings (volume, resolution, brightness)

---

## How to play

1. Download and unzip the Windows build from GitHub Releases
2. Launch the game via the executable (`CircusGame.exe`)
3. Choose characters, roll the dice, and reach the finish
4. Scores will be stored between sessions

Alternatively, the full Unity project can be opened using Unity 2022.3 or later.

---

## Repository structure

- `Assets/` – main project files and scripts
- `Build/` – Windows executable version (added in GitHub Releases)
- `README.md` – this file

---

## Version control

This project uses Git for version control.  
All development progress is committed and published in a GitHub repository. 

---

## To-do (progress checklist)

- [x] Create cursor script  
- [x] Add and animate UI  
- [x] Add background music and sounds  
- [x] Create animated characters  
- [x] Character selection screen  
- [x] Settings panel (brightness, resolution, volume)  
- [x] Dice roll mechanics  
- [x] Game board using tile-based levels  
- [x] Hurt, bridge, and chest tile logic  
- [x] Turn management  
- [x] Character movement on board using tiles  
- [x] Leaderboard with score calculation  
- [x] Scrollable ranking UI  
- [x] Save/load with JSON  
- [x] Score based on time and actions  
- [x] Pause menu  
- [x] Build executable  
- [ ] Fight animation when two players land on the same tile — *not implemented*  
- [ ] Display current game state and active player — *not implemented*  
- [ ] Info panel accessible through pause menu — *not implemented*

---

Created for the "Computer Game Development" course.  
Final build and project source uploaded to GitHub.
