namespace MyCalendar

open System

type Day =
    | First
    | Second
    | Third
    | Fourth
    | Fifth
    | Sixth
    | Seventh
    | Eighth
    | Ninth
    | Tenth
    | Eleventh
    | Twelfth
    | Thirteenth
    | Fourteenth
    | Fifteenth
    | Sixteenth
    | Seventeenth
    | Eighteenth
    | Nineteenth
    | Twentieth
    | Twentyfirst
    | Twentysecond
    | Twentythird
    | Twentyfourth
    | Twentyfifth
    | Twentysixth
    | Twentyseventh
    | Twentyeighth
    | Twentyninth
    | Thirtieth
    | Thirtyfirst

type Month =
    | January
    | February
    | March
    | April
    | May
    | June
    | July
    | August
    | September
    | October
    | November
    | December

type RecurringType =
    | EveryDay
    | EveryWeek of dayOfWeek: DayOfWeek
    | EveryMonth of day: Day
    | EveryYear of month: Month * day: Day

// Creating this struct since DateOnly native type is not handled by FSharp.Json
type OnlyDate =
    { Year: int
      Month: int
      Day: int }
    
    static member TryParse(input: string) =
        let success, result = DateOnly.TryParse(input)
        if success then
            Some { Year = result.Year; Month = result.Month; Day = result.Day }
        else
            None

type RecurringEvent =
    { Id: Guid
      Name: string
      Description: string
      IsImportant: bool
      RecurringType: RecurringType
      CreatedAt: DateTime
      SoftDeleted: bool }

type ToDo =
    { Id: Guid
      Name: string
      Description: string
      CreatedAt: DateTime
      MarkedDoneAt: DateTime option
      SoftDeleted: bool }

type Event =
    { Id: Guid
      Name: string
      Description: string
      IsImportant: bool
      When: OnlyDate
      CreatedAt: DateTime
      SoftDeleted: bool }

type MyCalendarData =
    { RecurringEvents: RecurringEvent array
      ToDos: ToDo array
      Events: Event array }

    static member Default =
        { RecurringEvents = Array.empty<RecurringEvent>
          ToDos = Array.empty<ToDo>
          Events = Array.empty<Event> }

[<RequireQualifiedAccess>]
module ToDo =

    let toString (todo: ToDo) = $"{todo.Name}: {todo.Description}"

    let private equalId (first: ToDo) (second: ToDo) = first.Id.Equals(second.Id)
        
    let update (todo: ToDo) (todos: ToDo array) =
        Array.tryFindIndex (equalId todo) todos
        |> function
            | None -> todos
            | Some index -> Array.updateAt index todo todos
            
    let active (todos: ToDo array) =
        todos
        |> Array.filter (fun td -> not td.SoftDeleted && Option.isNone td.MarkedDoneAt)
        |> Array.sortByDescending (fun td -> td.CreatedAt)
            
    let markedDone (todos: ToDo array) =
        todos
        |> Array.filter (fun td -> Option.isSome td.MarkedDoneAt)
        |> Array.sortByDescending (fun td -> td.CreatedAt)
            
    let deletable (todos: ToDo array) =
        todos
        |> Array.filter (fun td -> not td.SoftDeleted)
        |> Array.sortByDescending (fun td -> td.CreatedAt)

    let private markedDoneMoreThanDaysAgo (now: DateTime) (days: int) (todo: ToDo) =
        match todo.MarkedDoneAt with
        | None -> false
        | Some dateTime -> dateTime.Day + days <= now.Day

    let extractForView (now: DateTime) (todos: ToDo array) =
        let (markedDone, notMarkedDone) =
            todos
            |> Array.filter (fun td -> not td.SoftDeleted)
            |> Array.filter (markedDoneMoreThanDaysAgo now 5 >> not)
            |> Array.sortByDescending (fun td -> td.CreatedAt)
            |> Array.partition (fun td -> Option.isSome td.MarkedDoneAt)

        Array.append notMarkedDone markedDone

[<RequireQualifiedAccess>]
module Event =

    let toString (event: Event) = 
        let year = sprintf "%04i" event.When.Year
        let month = sprintf "%02i" event.When.Month
        let day = sprintf "%02i" event.When.Day
        $"{event.Name}: {event.Description} ({year}-{month}-{day})"

    let private equalId (first: Event) (second: Event) = first.Id.Equals(second.Id)
        
    let update (event: Event) (events: Event array) =
        Array.tryFindIndex (equalId event) events
        |> function
            | None -> events
            | Some index -> Array.updateAt index event events

    let nextEvents (now: DateTime) (events: Event array) =
        let nowAsOnlyDate = { Year = now.Year; Month = now.Month; Day = now.Day }

        events
        |> Array.filter (fun ev -> not ev.SoftDeleted)
        |> Array.filter (fun ev -> (compare nowAsOnlyDate ev.When) <= 0)
        |> Array.sortBy (fun ev -> ev.When)
        |> Array.truncate 5
