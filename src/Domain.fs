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
    { Name: string
      Description: string
      IsImportant: bool
      RecurringType: RecurringType
      CreatedAt: DateTime
      SoftDeleted: bool }

type ToDo =
    { Name: string
      Description: string
      CreatedAt: DateTime
      MarkedDoneAt: DateTime option
      SoftDeleted: bool }

type Event =
    { Name: string
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

  let private markedDoneMoreThanDaysAgo (now: DateTime) (days: int) (todo: ToDo) =
    match todo.MarkedDoneAt with
    | None -> false
    | Some dateTime -> 
      dateTime.Day + days <= now.Day 

  let extractForView (now: DateTime) (todos: ToDo array) =
    let (markedDone, notMarkedDone) =
      todos
      |> Array.filter (fun td -> not td.SoftDeleted)
      |> Array.filter (markedDoneMoreThanDaysAgo now 5 >> not)
      |> Array.sortBy (fun td -> td.CreatedAt)
      |> Array.partition (fun td -> Option.isSome td.MarkedDoneAt)
    
    Array.append notMarkedDone markedDone 