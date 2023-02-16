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
      When: DateOnly
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
