namespace MyCalendar

open System

type DayInWeek =
    | Sunday
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday

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
    | EveryWeek of dayInWeek: DayInWeek
    | EveryMonth of day: Day
    | EveryYear of month: Month * day: Day

    override self.ToString() =
        match self with
        | EveryWeek dayInWeek -> $"Every week on {dayInWeek}"
        | EveryMonth day -> $"Every month on {day}"
        | EveryYear(month, day) -> $"Every year on {day} of {month}"

// Creating this struct since DateOnly native type is not handled by FSharp.Json
type OnlyDate =
    { Year: int
      Month: int
      Day: int }

    static member TryParse(input: string) =
        let success, result = DateOnly.TryParse(input)

        if success then
            Some
                { Year = result.Year
                  Month = result.Month
                  Day = result.Day }
        else
            None

    override self.ToString() =
        let year = sprintf "%04i" self.Year
        let month = sprintf "%02i" self.Month
        let day = sprintf "%02i" self.Day
        $"{year}-{month}-{day}"

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
        $"{event.Name}: {event.Description} ({event.When.ToString()})"

    let private equalId (first: Event) (second: Event) = first.Id.Equals(second.Id)

    let update (event: Event) (events: Event array) =
        Array.tryFindIndex (equalId event) events
        |> function
            | None -> events
            | Some index -> Array.updateAt index event events

    let nextEvents (now: DateTime) (events: Event array) =
        let nowAsOnlyDate =
            { Year = now.Year
              Month = now.Month
              Day = now.Day }

        events
        |> Array.filter (fun ev -> not ev.SoftDeleted)
        |> Array.filter (fun ev -> (compare nowAsOnlyDate ev.When) <= 0)
        |> Array.sortBy (fun ev -> ev.When)
        |> Array.truncate 5
