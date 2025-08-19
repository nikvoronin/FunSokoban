# FunSokoban

Minimalistic `Fun`ctional `Sokoban` (console, no mutable vars). Initially vibe coded with ChatGPT 5. Brought to working condition by Human Being 45.

```plain
>
 ┏┓    ┏┓     ┓     
 ┣ ┓┏┏┓┗┓┏┓┏┏┓┣┓┏┓┏┓
 ┻ ┗┻┛┗┗┛┗┛┗┗┛┗┛┗┻┛┗
```

- [Interface](#interface)
- [Level Collections](#level-collections)
- [Develop F# Console Project](#develop-f-console-project)
  - [Create and Run](#create-and-run)
  - [Project Additionals](#project-additionals)
  - [Debugging (vscode)](#debugging-vscode)
  - [F5 Unexpected Behavior](#f5-unexpected-behavior)

```plain
> FunSokoban.exe
> FunSokoban.exe levels/microcosmos.xsb
```

## Interface

### Menu

- **Start Level** -- type level number then press `Enter` to start the selected level
- **Quit** (menu and game) -- press Enter with no input, or enter a value outside the available range.

```plain
 ┏┓    ┏┓     ┓
 ┣ ┓┏┏┓┗┓┏┓┏┏┓┣┓┏┓┏┓
 ┻ ┗┻┛┗┗┛┗┛┗┗┛┗┛┗┻┛┗
 Functional..Sokoban

  1.  Level 1 – Simple push
  2.  Level 2 – Two boxes, two goals
  3.  Level 3 – Narrow corridor
  4.  Level 4 – Small warehouse
  5.  Level 5 – Lazy house
  6.  Level 6 – Out of bounds

 Choose a level number [1..6] (Enter): 4
```

### In-game

- `←↑↓→` move // arrows, cursor keys
- `U` undo moves
- `R` restart level
- `N` start next level
- `P` start previous level
- `Q` quit game

```plain
🧱🧱🧱🧱🧱🧱🧱🧱
🧱            🧱
🧱  😊🎯  ✅  🧱
🧱    📦      🧱
🧱      🧱🧱🧱
🧱🧱🧱🧱🧱

Level:  Level 4 – Small warehouse | Moves: 17
←↑↓→ to move :: U undo | R restart | N next | P prev | Q quit
```

## Level Collections

- Empty lines are ignored.
- Level name starts with `;` symbol. One line for level name.
- Grid symbols
  - `#` Wall
  - `.` Goal plate
  - `$` Box
  - `*` Box on goal plate
  - `@` Player
  - `+` Player on goal plate
  - Ignore anything else

```plain
; Level 1 – Two boxes, two goals

#######
#  . .#
#  $  #
# @$  #
#_____#
#######


; Level 2 – Narrow corridor

########
#@     #
# $##  #
# *.  #
########
```

### Level Packs

Level data is loaded from text files, see the `levels/*` directory. The game reads levels in a format based on the standard Sokoban `.xsb` file format.

- [Levels](levels/levels.txt) (6) - test levels for this project
- [Microban](levels/sb_microban_1.xsb) (155) - David W. Skinner ©️ 2000
- [Microcosmos](levels/Microcosmos.xsb) (40) - Aymeric du Peloux ©️ 1996-2000
- [Minicosmos](levels/Minicosmos.xsb) (40) - Aymeric du Peloux ©️ 2001

## Develop F# Console Project

### Create and Run

```plain
> dotnet new console -lang F# -o Sokofun
> dotnet build
> dotnet run
```

### Project Additionals

- Copy levels files to output folder
- Clean up garbage with all possible langauge satelite assemblies.

```xml
<Project Sdk="Microsoft.NET.Sdk">

...

  <ItemGroup>
    <Content Include="levels/*.*">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

  <PropertyGroup>
    <SatelliteResourceLanguages>en-US</SatelliteResourceLanguages>
  </PropertyGroup>
</Project>
```

### Debugging (vscode)

Inside `.vscode` folder, we should create two files: `launch.json` and `tasks.json`

#### launch.json

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "F# Launch (console)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/FunSokoban.dll",
      "args": [
        "levels/levels.txt",
        //"levels/sb_microban_1.xsb"
      ],
      "cwd": "${workspaceFolder}",
      "console": "integratedTerminal",
      "stopAtEntry": false,
      "justMyCode": true
    }
  ]
}
```

#### tasks.json

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "type": "process",
      "command": "dotnet",
      "args": [
        "build", 
        "${workspaceFolder}/${workspaceFolderBasename}.fsproj" 
      ],
      "problemMatcher": "$msCompile",
      "group": "build"
    }
  ]
}
```

### F5 Unexpected Behavior

If you encounter issues with running, debugging, or seeing output in the VS Code internal terminal, proceed with these steps:

- Open keyboard shortcut: `Ctrl + K` `Ctrl + S`
- Search for `F5`
- Unset the `debug.openView` by right-click & `remove keybinding`
- Do the same for `F# Debug Default Project`
