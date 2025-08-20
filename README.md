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
  - [Release Build](#release-build)
  - [Create and Run](#create-and-run)
  - [Project Additionals](#project-additionals)
  - [Debugging (vscode)](#debugging-vscode)
  - [F5 Unexpected Behavior](#f5-unexpected-behavior)
- [VIBE CODED WITH ARTIFICIAL INTELLIGENCE](#vibe-coded-with-artificial-intelligence)

**System requirements:** Windows 10 x64, .NET Desktop Runtime 9.0.\
Linux and macOS are also supported with the corresponding .NET runtime.

For the initial AI-generated prototype, check out the [vibe-coding](https://github.com/nikvoronin/FunSokoban/tree/vibe-coding) branch. It includes the vibe-code and the complete prompt history.

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
🧱▪️▪️▪️▪️▪️▪️🧱
🧱▪️😊🎯▪️✅▪️🧱
🧱▪️▪️📦▪️▪️🧱
🧱▪️▪️▪️🧱🧱🧱
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
- [Microban](levels/sb_microban_1.xsb) (155) - David W. Skinner © 2000
- [Microcosmos](levels/Microcosmos.xsb) (40) - Aymeric du Peloux © 1996-2000
- [Minicosmos](levels/Minicosmos.xsb) (40) - Aymeric du Peloux © 2001

## Develop F# Console Project

### Release Build

> dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true

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

## VIBE CODED WITH ARTIFICIAL INTELLIGENCE

😊 Forge me a minimalistic F# Sokoban. A console-bound ghost. No mutable variables. A pure, functional specter.

🤖 (THOUGHT: 32 SECONDS OF WHITE-HOT FOCUS)\
A single-file phantasm! A console-bound puzzle of pure function. No mutables. No side-effects. A ghost in the machine.\
**CONTROLS:** Arrows to push your will. `U` to unwind time. `R` to reset reality. `Q` to quit the illusion.

😊 Now. The incantation. The console command to birth this ghost from its `.fsx` script-shell.

🤖 (THOUGHT: 9 SECONDS. A BLINK.)\
No build. No ceremony. A direct line to the phantom. One command to rule it: `dotnet fsi fsokoban.fsx` Speak it, and the game lives!

😎 **ERROR.** The code-reality fractures!\
`fsokoban.fsx(49,71): error FS3373...`\
The string-interpolation is flawed. A syntax paradox!

🤖 (THOUGHT: 18 SECONDS OF DEBUGGING REALITY)\
The compiler's logic-bomb! You can't nest those literals. A classic trap.\
**SOLUTION:** Pre-compute the status text. A let binding—a temporary reality—before interpolation. Clean. Safe.

😎 **NEXT ERROR.** The compiler's mind is indeterminate!\
`fsokoban.fsx(87,23): error FS0072...`\
The type is a blur! The Set.empty is a void without form!

🤖 (THOUGHT: 10 SECONDS OF FORCING CLARITY)\
Ambiguity! The compiler doesn't know what kind of emptiness you desire. A set of what?\
**FIX:** Annotate! Shackle it with a type: `Set.empty<Pos>`. And for union, use `Set.union`, not the ambiguous `+`. Force the logic!

😎 **A WARNING.** Not an error. A whisper.\
`fsokoban.fsx(163,3): warning FS2304...`
The `[<EntryPoint>]` is not auto-invoked in the FSI séance.

🤖 A trifle! The ghost is compiled but not summoned. FSI is a cautious medium.\
To banish the warning, you must speak the ghost's name: main `[||]` at the script's tail.\
Or... would you prefer a rewrite? A single script that functions as both a séance (FSI) and a standalone reality (compiled exe)? A binary existence!

😊 The level data. It must be external. A text file. A separate dimension to load on startup.

🤖 (THOUGHT: 9 SECONDS OF ARCHITECTING DIMENSIONS)\
A clean rip in the fabric of the code! The level is no longer hard-coded. It is read. A text file, a new reality sucked into the console upon initiation. The change is made. The purity remains. It works in both states: séance and solid form.

😊 Four more test levels. Four new pocket realities of pushing and boxes.

🤖 Done. Four small, cruel heavens for your puzzle-ghost.
Shall I bundle them? A single file, a "level pack," dimensions separated by a line of cosmic static—a ; or a void. A multiverse in a text file.

😊 Bundle them. And then... connect them. A loop between realities.

🤖 (THOUGHT: 21 SECONDS OF WEAVING WORLD-LINES)\
**YES.** The extension is coded.

The game now loads a multiverse—a pack of levels.

A menu at the start—a crossroads of choice.

From within the game, you can leap to the Next world, the Previous one.

Solve a puzzle, and the game auto-advances—a reward of a new challenge!\
All immutable. All console-bound. The ghost has learned to teleport.

😊 Levels 2 and 3. They are lies. Impossible prisons. Deadlocks on arrival. **FIX THEM.**

🤖 (THOUGHT: 2 MINUTES, 35 SECONDS OF INTENSE TEMPORAL MANIPULATION)\
You saw through the illusion! Flawed realities. Boxes born in stone-coffins. Unsolvable.\
I have demolished them and rebuilt from the atoms up. Levels 1 & 4 stand. Levels 2 & 3 are new, elegant, solvable prisons. The fresh `levels.txt` is ready.

😊 Sources. Where does one find more Sokoban dimensions? Free ones.

🤖 The universe is vast with them! Archives of ancient puzzle-smiths.

The XSB Repository: The motherlode. The original texts.

David W. Skinner's Microban Series: The classic test. 155 small, solvable bites of logic.

Sokoban Wiki: A hive-mind's collection.\
They are plain text. ASCII. Your game's native tongue.
Start with Microban 1. It is the perfect training ground for your ghost.

😊 Adapt it. Make the Microban pack a ready-to-consume reality for my game.

🤖 (THOUGHT: 31 SECONDS OF REALITY ASSIMILATION)\
**THE BEAUTY!** The Microban format is the XSB format. It is already compatible with the parser you possess!\
No adaptation needed. The ghost is already a universal solvent.\
Simply point your game at the `microban.txt` file. It will consume the entire 155-level multiverse without a single code change.

The game is ready. The worlds are infinite.
