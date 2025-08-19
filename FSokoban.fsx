// FSokoban.fs — minimalistic functional Sokoban (console, no mutable vars)
// Controls (in-game): Arrows move | U undo | R restart | N next | P prev | Q quit
// Menu: Up/Down to select, Enter to start, Esc to quit

open System
open System.IO

// ---------- Domain ----------
type Pos = int * int

type State = {
    Player : Pos
    Boxes : Set<Pos>
    Walls : Set<Pos>
    Goals : Set<Pos>
    Width : int
    Height : int
    Moves : int
}

let inline add (x1, y1) (x2, y2) =
    (x1 + x2, y1 + y2)

let isSolved (state: State) =
    state.Boxes = state.Goals

// ---------- Parsing ----------
let parse (levelText: string) : State =
    // Normalize line endings and keep non-empty lines
    let lines =
        levelText
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split('\n')
        |> Array.toList
        |> List.filter (fun s -> s.Length > 0)
    
    if List.isEmpty lines 
    then failwith "Level is empty."
    
    let w =
        lines 
        |> List.map (fun s -> s.Length) 
        |> List.max
    let h = lines.Length

    // Fold through characters to collect sets/positions (type-annotated to avoid FS0072)
    let folder (x, y, (walls:Set<Pos>,goals:Set<Pos>,boxes:Set<Pos>,playerOpt: Pos option)) (ch:char) =
        let p = (x,y)
        match ch with
        | '#' -> (x+1,y, (walls.Add p, goals, boxes, playerOpt))
        | '.' -> (x+1,y, (walls, goals.Add p, boxes, playerOpt))
        | '$' -> (x+1,y, (walls, goals, boxes.Add p, playerOpt))
        | '*' -> (x+1,y, (walls, goals.Add p, boxes.Add p, playerOpt))
        | '@' -> (x+1,y, (walls, goals, boxes, Some p))
        | '+' -> (x+1,y, (walls, goals.Add p, boxes, Some p))
        | _     -> (x+1,y, (walls, goals, boxes, playerOpt))

    let lineFolder (y,(walls:Set<Pos>,goals:Set<Pos>,boxes:Set<Pos>,playerOpt)) (s:string) =
        let (_,_,(w2,g2,b2,p2)) =
            s |> Seq.fold folder (0,y,(Set.empty<Pos>, Set.empty<Pos>, Set.empty<Pos>, playerOpt))
        (y+1,(Set.union walls w2, Set.union goals g2, Set.union boxes b2, p2))

    let (_, (walls,goals,boxes,playerOpt)) =
        lines |> List.fold lineFolder (0,(Set.empty<Pos>,Set.empty<Pos>,Set.empty<Pos>,None))

    let player =
        match playerOpt with
        | Some p -> p
        | None -> failwith "Level must contain a player '@' or '+'."

    {   Player = player
        Boxes  = boxes
        Walls  = walls
        Goals  = goals
        Width  = w
        Height = h
        Moves  = 0 }

// ---------- Rendering ----------
let render (levelName:string) (state:State) =
    Console.Clear()
    let charAt p =
        let onWall    = state.Walls.Contains p
        let onGoal    = state.Goals.Contains p
        let onBox     = state.Boxes.Contains p
        let onPlayer= state.Player = p
        if onWall then '#'
        elif onBox && onGoal then '*'
        elif onBox then '$'
        elif onPlayer && onGoal then '+'
        elif onPlayer then '@'
        elif onGoal then '.'
        else ' '
    // Grid
    [0 .. state.Height-1]
    |> List.iter (fun y ->
            [0 .. state.Width-1]
            |> List.map (fun x -> charAt (x,y))
            |> Array.ofList
            |> fun arr -> Console.WriteLine(String arr))
    // HUD (avoid FS3373 by building strings separately)
    Console.WriteLine()
    let status =
        if isSolved state
        then "Solved! (Enter/N next, P prev, R restart, Q quit)"
        else "Arrows move | U undo | R restart | N next | P prev | Q quit"
    Console.WriteLine(sprintf "Level: %s | Moves: %d     %s" levelName state.Moves status)

// ---------- Rules ----------
let tryMove (dir:Pos) (state:State) : State =
    let p1 = add state.Player dir
    let p2 = add p1 dir
    let push =
        state.Boxes.Contains p1
        && not (state.Walls.Contains p2)
        && not (state.Boxes.Contains p2)
    let walk = not (state.Walls.Contains p1) && not (state.Boxes.Contains p1)
    if push then
        { state with
                Player = p1
                Boxes    = state.Boxes.Remove p1 |> fun b -> b.Add p2
                Moves    = state.Moves + 1 }
    elif walk then
        { state with Player = p1; Moves = state.Moves + 1 }
    else state

// ---------- Input ----------
let readDir (k:ConsoleKey) : Pos option =
    match k with
    | ConsoleKey.LeftArrow    -> Some (-1,0)
    | ConsoleKey.RightArrow -> Some ( 1,0)
    | ConsoleKey.UpArrow        -> Some ( 0,-1)
    | ConsoleKey.DownArrow    -> Some ( 0, 1)
    | _ -> None

// ---------- Level pack splitting ----------
type LevelDef = { Title : string; Text : string }

let splitLevels (packText:string) : LevelDef[] =
    let normalized =
        packText.Replace("\r\n","\n").Replace("\r","\n")
    // Split on 1+ blank lines
    let rawBlocks =
        normalized.Split([|"\n\n"|], StringSplitOptions.RemoveEmptyEntries)
    let cleanBlock (raw:string) =
        let lines = raw.Split('\n') |> Array.toList
        let title =
            lines
            |> List.tryFind (fun s -> s.TrimStart().StartsWith(";"))
            |> Option.map (fun s -> s.TrimStart().TrimStart(';').Trim())
            |> Option.defaultValue "Untitled level"
        let cleaned =
            lines
            |> List.filter (fun s -> not (s.TrimStart().StartsWith(";")))
            |> fun xs ->
                    xs
                    |> List.skipWhile (fun s -> s.Trim() = "")
                    |> List.rev
                    |> List.skipWhile (fun s -> s.Trim() = "")
                    |> List.rev
            |> String.concat "\n"
        { Title = title; Text = cleaned }
    rawBlocks |> Array.map cleanBlock |> Array.filter (fun ld -> ld.Text.Trim() <> "")

// ---------- Menu (level choice) ----------
let renderMenu (levels:LevelDef[]) selected =
    Console.Clear()
    Console.WriteLine("Sokoban — Choose a level (↑/↓ to move, Enter to start, Esc to quit)")
    Console.WriteLine()
    levels
    |> Array.mapi (fun i lvl ->
            let mark = if i = selected then ">" else " "
            sprintf " %s %2d. %s" mark (i+1) lvl.Title)
    |> Array.iter Console.WriteLine

let rec chooseLevel (levels:LevelDef[]) selected =
    renderMenu levels selected
    match Console.ReadKey(true).Key with
    | ConsoleKey.UpArrow     ->
            let s = if selected <= 0 then levels.Length-1 else selected-1
            chooseLevel levels s
    | ConsoleKey.DownArrow ->
            let s = if selected >= levels.Length-1 then 0 else selected+1
            chooseLevel levels s
    | ConsoleKey.Home            -> chooseLevel levels 0
    | ConsoleKey.End             -> chooseLevel levels (levels.Length-1)
    | ConsoleKey.Escape        -> None
    | ConsoleKey.Enter         -> Some selected
    | _                                        -> chooseLevel levels selected

// ---------- Per-level loop (pure, no mutable) ----------
type LevelSignal = Prev | Next | Quit

let rec loopLevel (levelName:string) (initial:State) : LevelSignal =
    let rec go (state:State) (history:State list) : LevelSignal =
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
let tryIndex (levels:LevelDef[]) i =
    if i < 0 then None elif i >= levels.Length then None else Some i

let rec loopPack (levels:LevelDef[]) current =
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
let getArgs () =
#if INTERACTIVE
    fsi.CommandLineArgs |> Array.skip 1
#else
    Environment.GetCommandLineArgs() |> Array.skip 1
#endif

let chooseLevelPath () =
    let args = getArgs ()
    if args.Length >= 1 then args.[0] else "levels.txt"

// ---------- Run ----------
let run () =
    Console.OutputEncoding <- System.Text.Encoding.UTF8
    Console.CursorVisible <- false
    try
        let levelPath = chooseLevelPath ()
        let packText    = File.ReadAllText levelPath
        let levels        = splitLevels packText
        if levels.Length = 0 then failwith "No levels found in the file."
        match chooseLevel levels 0 with
        | None -> () // Esc at menu
        | Some startIndex -> loopPack levels startIndex
    with ex ->
        Console.WriteLine("Error: " + ex.Message)
        Console.WriteLine("Usage: dotnet fsi Sokoban.fsx [levels-file]")
        Console.WriteLine("Or when compiled: dotnet run -- [levels-file]")
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey(true) |> ignore

#if INTERACTIVE
run ()
#else
[<EntryPoint>]
let main _ =
    run ()
    0
#endif
