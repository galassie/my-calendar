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

    static member ToDayOfWeek(dayInWeek: DayInWeek) =
        match dayInWeek with
        | Sunday -> DayOfWeek.Sunday
        | Monday -> DayOfWeek.Monday
        | Tuesday -> DayOfWeek.Tuesday
        | Wednesday -> DayOfWeek.Wednesday
        | Thursday -> DayOfWeek.Thursday
        | Friday -> DayOfWeek.Friday
        | Saturday -> DayOfWeek.Saturday

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

    static member ToDayInt(day: Day) =
        match day with
        | First -> 1
        | Second -> 2
        | Third -> 3
        | Fourth -> 4
        | Fifth -> 5
        | Sixth -> 6
        | Seventh -> 7
        | Eighth -> 8
        | Ninth -> 9
        | Tenth -> 10
        | Eleventh -> 11
        | Twelfth -> 12
        | Thirteenth -> 13
        | Fourteenth -> 14
        | Fifteenth -> 15
        | Sixteenth -> 16
        | Seventeenth -> 17
        | Eighteenth -> 18
        | Nineteenth -> 19
        | Twentieth -> 20
        | Twentyfirst -> 21
        | Twentysecond -> 22
        | Twentythird -> 23
        | Twentyfourth -> 24
        | Twentyfifth -> 25
        | Twentysixth -> 26
        | Twentyseventh -> 27
        | Twentyeighth -> 28
        | Twentyninth -> 29
        | Thirtieth -> 30
        | Thirtyfirst -> 31

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

    static member ToMonthInt(month: Month) =
        match month with
        | January -> 1
        | February -> 2
        | March -> 3
        | April -> 4
        | May -> 5
        | June -> 6
        | July -> 7
        | August -> 8
        | September -> 9
        | October -> 10
        | November -> 11
        | December -> 12

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

    static member IsEveryWeekType(event: RecurringEvent) =
        match event.RecurringType with
        | EveryWeek _ -> true
        | EveryMonth _ -> false
        | EveryYear _ -> false

    static member IsEveryMonthType(event: RecurringEvent) =
        match event.RecurringType with
        | EveryWeek _ -> false
        | EveryMonth _ -> true
        | EveryYear _ -> false

    static member IsEveryYearType(event: RecurringEvent) =
        match event.RecurringType with
        | EveryWeek _ -> false
        | EveryMonth _ -> false
        | EveryYear _ -> true

    override self.ToString() =
        $"{self.Name}: {self.Description} ({self.RecurringType.ToString()})"

type ToDo =
    { Id: Guid
      Name: string
      Description: string
      CreatedAt: DateTime
      MarkedDoneAt: DateTime option
      SoftDeleted: bool }

    override self.ToString() = $"{self.Name}: {self.Description}"

type Event =
    { Id: Guid
      Name: string
      Description: string
      IsImportant: bool
      When: OnlyDate
      CreatedAt: DateTime
      SoftDeleted: bool }

    override self.ToString() =
        $"{self.Name}: {self.Description} ({self.When.ToString()})"

type MyCalendarData =
    { RecurringEvents: RecurringEvent array
      ToDos: ToDo array
      Events: Event array }

    static member Default =
        { RecurringEvents = Array.empty<RecurringEvent>
          ToDos = Array.empty<ToDo>
          Events = Array.empty<Event> }

[<RequireQualifiedAccess>]
module Constants =

    let maxCount = 5

[<RequireQualifiedAccess>]
module ToDo =

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

    let getViewable (now: DateTime) (todos: ToDo array) =
        let (markedDone, notMarkedDone) =
            todos
            |> Array.filter (fun td -> not td.SoftDeleted)
            |> Array.filter (markedDoneMoreThanDaysAgo now 5 >> not)
            |> Array.sortByDescending (fun td -> td.CreatedAt)
            |> Array.partition (fun td -> Option.isSome td.MarkedDoneAt)

        Array.append notMarkedDone markedDone

[<RequireQualifiedAccess>]
module Event =

    let private equalId (first: Event) (second: Event) = first.Id.Equals(second.Id)

    let update (event: Event) (events: Event array) =
        Array.tryFindIndex (equalId event) events
        |> function
            | None -> events
            | Some index -> Array.updateAt index event events

    let nextEvents (maxCount: int) (now: DateTime) (events: Event array) =
        let nowAsOnlyDate =
            { Year = now.Year
              Month = now.Month
              Day = now.Day }

        events
        |> Array.filter (fun evt -> not evt.SoftDeleted)
        |> Array.filter (fun evt -> (compare nowAsOnlyDate evt.When) <= 0)
        |> Array.sortBy (fun evt -> evt.When)
        |> Array.truncate maxCount

[<RequireQualifiedAccess>]
module RecurringEvent =

    let private equalId (first: RecurringEvent) (second: RecurringEvent) = first.Id.Equals(second.Id)

    let update (event: RecurringEvent) (events: RecurringEvent array) =
        Array.tryFindIndex (equalId event) events
        |> function
            | None -> events
            | Some index -> Array.updateAt index event events

    let private mapToEvents (maxCount: int) (now: DateTime) (event: RecurringEvent) =
        match event.RecurringType with
        | RecurringType.EveryWeek dayInWeek ->
            [| for i in 0 .. (maxCount - 1) ->
                   let daysToAdd =
                       (((int) (DayInWeek.ToDayOfWeek dayInWeek) - (int) now.DayOfWeek + 7) % 7)
                       + (i * 7)

                   let nextWhen = now.AddDays(daysToAdd)

                   { Id = Guid.Empty
                     Name = event.Name
                     Description = event.Description
                     IsImportant = event.IsImportant
                     When =
                       { Year = nextWhen.Year
                         Month = nextWhen.Month
                         Day = nextWhen.Day }
                     CreatedAt = event.CreatedAt
                     SoftDeleted = event.IsImportant } |]
        | RecurringType.EveryMonth day ->
            let dayInt = Day.ToDayInt day
            let includeThisMonthOrNot = if dayInt >= now.Day then 0 else 1

            [| for i in 0 .. (maxCount - 1) ->
                   let monthsToAdd = includeThisMonthOrNot + i
                   let nextWhen = now.AddMonths(monthsToAdd)

                   { Id = Guid.Empty
                     Name = event.Name
                     Description = event.Description
                     IsImportant = event.IsImportant
                     When =
                       { Year = nextWhen.Year
                         Month = nextWhen.Month
                         Day = dayInt }
                     CreatedAt = event.CreatedAt
                     SoftDeleted = event.IsImportant } |]
        | RecurringType.EveryYear(month, day) ->
            let monthInt = Month.ToMonthInt month
            let dayInt = Day.ToDayInt day

            let includeThisYearOrNot =
                if (monthInt > now.Month) || (monthInt = now.Month && dayInt >= now.Day) then
                    0
                else
                    1

            [| for i in 0 .. (maxCount - 1) ->
                   let yearsToAdd = includeThisYearOrNot + i
                   let nextWhen = now.AddYears(yearsToAdd)

                   { Id = Guid.Empty
                     Name = event.Name
                     Description = event.Description
                     IsImportant = event.IsImportant
                     When =
                       { Year = nextWhen.Year
                         Month = monthInt
                         Day = dayInt }
                     CreatedAt = event.CreatedAt
                     SoftDeleted = event.IsImportant } |]

    let generateEvents (maxCount: int) (now: DateTime) (events: RecurringEvent array) =
        events
        |> Array.filter (fun evt -> not evt.SoftDeleted)
        |> Array.map (mapToEvents maxCount now)
        |> Array.fold Array.append [||]
        |> Array.sortBy (fun evt -> evt.When)
        |> Array.truncate maxCount

    let getViewable (maxCount: int) (events: RecurringEvent array) =
        events
        |> Array.filter (fun evt -> not evt.SoftDeleted)
        |> Array.sortBy (fun evt -> evt.CreatedAt)
        |> Array.truncate maxCount
