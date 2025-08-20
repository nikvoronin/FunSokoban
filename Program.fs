(*

o--o            o-o               o            
|              |                  |            
O-o  o  o o-o   o-o  o-o  o-o o-o O-o   oo o-o 
|    |  | |  |     | | | |    | | |  | | | |  |
o    o--o o  o o--o  o-o  o-o o-o o-o  o-o-o  o

FunSokoban -- Minimalistic functional Sokoban (console, no mutable vars)
Initially vibe coded with ChatGPT 5
Brought to working condition by Human Being 45
Controls (in-game): Arrows move | U undo | R restart | N next | P prev | Q quit
Menu: Enter to select a level, empty string or out of range value to quit.

*)

open System
open System.IO

// ---------- Domain ----------
type Pos = int * int

type World = {
    Boxes : Set<Pos>
    Walls : Set<Pos>
    Goals : Set<Pos>
    Player: Pos
}

type State = {
    Grid: World
    Width : int
    Height : int
    Moves : int
}

let NonePos = -1, -1

let inline add (x1, y1) (x2, y2) =
    x1 + x2, y1 + y2

let isSolved (state: State) =
    state.Grid.Boxes = state.Grid.Goals

// ---------- Parsing ----------
let parse (levelText: string) =
    // Normalize line endings and keep non-empty lines
    let lines =
        levelText
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split '\n'
        |> Array.toList
        |> List.filter (fun s -> s.Length > 0)
    
    if List.isEmpty lines 
    then failwith "Level is empty."
    
    let w =
        lines 
        |> List.map (fun s -> s.Length) 
        |> List.max
    let h = lines.Length

    // Fold through characters to collect sets/positions
    let folder (x, y, w: World) ch =
        let p = x, y

        match ch with
        | '#' -> x + 1, y, { w with Walls = w.Walls.Add p } // Wall
        | '.' -> x + 1, y, { w with Goals = w.Goals.Add p } // Goal plate
        | '$' -> x + 1, y, { w with Boxes = w.Boxes.Add p } // Box
        | '*' ->                                            // Box on goal plate
            x + 1, y, 
            { w with
                Boxes = w.Boxes.Add p 
                Goals = w.Goals.Add p
            }
        | '@' -> x + 1, y, { w with Player = p }            // Player
        | '+' ->                                            // Player on goal plate
            x + 1, y,
            { w with
                Player = p
                Goals = w.Goals.Add p 
            }
        | _   -> x + 1, y, w                                // Ignore anything else

    let lineFolder (y, w: World) s =
        let _, _, w2 =
            s 
            |> Seq.fold folder 
                ( 0, y,
                    { Boxes = Set.empty<Pos>
                    ; Walls = Set.empty<Pos>
                    ; Goals = Set.empty<Pos>
                    ; Player = w.Player
                    }
                )

        y + 1,
        { Walls = Set.union w.Walls w2.Walls
        ; Goals = Set.union w.Goals w2.Goals
        ; Boxes = Set.union w.Boxes w2.Boxes
        ; Player = w2.Player
        }

    let _, world =
        lines 
        |> List.fold lineFolder (
            0,
            { Walls = Set.empty<Pos>
            ; Goals = Set.empty<Pos>
            ; Boxes = Set.empty<Pos>
            ; Player = NonePos
            }
        )

    let player =
        match world.Player with
        | p when p = NonePos ->
            failwith "Level must contain a player '@' or '+'"
        | _ -> world.Player

    {   Grid = {
            Player = player
            Boxes  = world.Boxes
            Walls  = world.Walls
            Goals  = world.Goals
        }
        Width  = w
        Height = h
        Moves  = 0
    }

// ---------- Rendering ----------
let render levelName (state: State) =
    Console.Clear()

    let charAt p =
        let onSolved = isSolved state
        let onWall = state.Grid.Walls.Contains p
        let onGoal = state.Grid.Goals.Contains p
        let onBox = state.Grid.Boxes.Contains p
        let onBoxOnGoal = onBox && onGoal
        let onPlayer = state.Grid.Player = p
        let onPlayerOnGoal = onPlayer && onGoal
        let onPlayerWhenSolved = onPlayer && onSolved

        if   onWall then "🧱"
        elif onBoxOnGoal then "✅"
        elif onBox then "📦"
        elif onPlayerOnGoal then "😎"
        elif onPlayerWhenSolved then "😁"
        elif onPlayer then "😊"
        elif onGoal then "🎯"
        else "💠"
        // if   onWall then '#'
        // elif onBoxOnGoal then '*'
        // elif onBox then '$'
        // elif onPlayerOnGoal then '+'
        // elif onPlayer then '@'
        // elif onGoal then '.'
        // else ' '
        
    // Grid
    [0 .. state.Height-1]
    |> List.iter 
        (fun y ->
            [0 .. state.Width - 1]
            |> List.map (fun x -> charAt (x, y))
            |> Array.ofList
            |> fun arr -> Console.WriteLine( String.Concat arr )
        )

    let status =
        if isSolved state
        then "\n🎉 Solved! :: Enter/N next | P prev | R restart | Q quit"
        else "\n←↑↓→ to move :: U undo | R restart | N next | P prev | Q quit"

    printfn ""
    printfn "Level: %s | Moves: %d     %s"
        levelName
        state.Moves
        status

// ---------- Rules ----------
let tryMove dir (state: State) =
    let p1 = add state.Grid.Player dir
    let p2 = add p1 dir
    let push =
        state.Grid.Boxes.Contains p1
        && not (state.Grid.Walls.Contains p2)
        && not (state.Grid.Boxes.Contains p2)
    let walk =
        not (state.Grid.Walls.Contains p1) 
        && not (state.Grid.Boxes.Contains p1)
    if push then
        { state with
            Grid = {
                state.Grid with 
                    Player = p1
                    Boxes =
                        state.Grid.Boxes.Remove p1
                        |> fun b -> b.Add p2
            }
            Moves = state.Moves + 1
        }
    elif walk then
        { state with
            Grid = { state.Grid with Player = p1 }
            Moves = state.Moves + 1 
        }
    else state

// ---------- Input ----------
let readDir k =
    match k with
    | ConsoleKey.LeftArrow -> Some (-1,0)
    | ConsoleKey.RightArrow -> Some ( 1,0)
    | ConsoleKey.UpArrow -> Some ( 0,-1)
    | ConsoleKey.DownArrow -> Some ( 0, 1)
    | _ -> None

// ---------- Level pack splitting ----------
type LevelDef = {
    Title : string
    Text : string 
}

let splitLevels (packText: string) =
    let normalized =
        packText
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")

    let rawBlocks =
        normalized.Split( ';', StringSplitOptions.RemoveEmptyEntries )

    let cleanBlock (raw: string) =
        let lines =
            raw.Split '\n' 
            |> Array.toList

        let title = List.head lines

        let cleaned =
            lines
            |> List.skip 1 // skip title
            |> List.skipWhile (fun s -> s.Trim() = "")
            |> String.concat "\n"

        {   Title = title
            Text = cleaned
        }

    rawBlocks 
    |> Array.map cleanBlock 
    |> Array.filter (fun ld -> ld.Text.Trim() <> "")

// ---------- Menu (level choice) ----------
let renderMenu levels selected =
    Console.Clear()

    printfn " ┏┓    ┏┓     ┓     "
    printfn " ┣ ┓┏┏┓┗┓┏┓┏┏┓┣┓┏┓┏┓" 
    printfn " ┻ ┗┻┛┗┗┛┗┛┗┗┛┗┛┗┻┛┗"
    printfn " Functional..Sokoban"
    printfn ""

    levels
    |> Array.mapi 
        (fun i lvl ->
            sprintf " %2d. %s" (i + 1) lvl.Title
        )
    |> Array.iter Console.WriteLine

    printfn ""
    printf  " Choose a level number [1..%d] (Enter): " (Array.length levels)

let rec chooseLevel levels selected =
    renderMenu levels selected

    let isNumInRange num lvls =
        num > 0 && num <= Array.length lvls

    // Level numbering starts from 1
    match Int32.TryParse(Console.ReadLine()) with
    | true, num when isNumInRange num levels ->
        Some (num - 1)
    | _ -> None

// ---------- Per-level loop (pure, no mutable) ----------
type LevelSignal = Prev | Next | Quit

let rec loopLevel levelName initial =
    let rec go state history =
        render levelName state

        let key = Console.ReadKey(true).Key
        match key with
        | ConsoleKey.Q -> Quit
        | ConsoleKey.R -> go initial []
        | ConsoleKey.U ->
            match history with
            | prev::rest -> go prev rest
            | [] -> go state history
        | ConsoleKey.N -> Next
        | ConsoleKey.P -> Prev
        | k ->
            match readDir k with
            | Some dir ->
                let next = tryMove dir state
                if next = state then go state history
                else go next (state::history)
            | None ->
                if isSolved state then
                    match k with
                    | ConsoleKey.Enter -> Next
                    | _ -> go state history
                else
                    go state history
    go initial []

// ---------- Pack loop ----------
let tryIndex (levels: LevelDef array) i =
    if i < 0 then None
    elif i >= levels.Length then None
    else Some i

let rec loopPack levels current =
    match tryIndex levels current with
    | None -> ()
    | Some i ->
        let lvl = levels.[i]
        let initial = parse lvl.Text
        match loopLevel lvl.Title initial with
        | Prev ->
            let prev = if i > 0 then i - 1 else levels.Length - 1
            loopPack levels prev
        | Next ->
            let next = if i < levels.Length - 1 then i + 1 else 0
            loopPack levels next
        | Quit -> ()

// ---------- Startup utilities ----------
let chooseLevelPath () =
    let args =
        Environment.GetCommandLineArgs()
        |> Array.skip 1
    
    if args.Length >= 1
    then args.[0] else "levels/levels.txt"

// ---------- Run ----------
let run () =
    Console.OutputEncoding <- Text.Encoding.UTF8
    Console.CursorVisible <- false
    try
        let levelPath = chooseLevelPath ()
        let packText = File.ReadAllText levelPath
        let levels = splitLevels packText

        if levels.Length = 0
        then failwith "No levels found in the file."

        match chooseLevel levels 0 with
        | None -> ()
        | Some startIndex ->
            loopPack levels startIndex
    with ex ->
        printfn $"Error: {ex.Message}"
        printfn "FunSokoban [levels-file-name.xsb]"
        printfn "Press any key to exit..."
        Console.ReadKey true |> ignore

[<EntryPoint>]
let main _ =
    run ()
    0
