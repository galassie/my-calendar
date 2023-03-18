namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module RecurringEventHandler =
    type private RecurringTypePromptSelection =
        | EveryWeek
        | EveryMonth
        | EveryYear

        override self.ToString() =
            match self with
            | EveryWeek -> "Every week"
            | EveryMonth -> "Every month"
            | EveryYear -> "Every year"

    let private dayInWeekPrompt () =
        selectionPrompt<DayInWeek> {
            title "Which day of a week?"

            choices
                [| DayInWeek.Sunday
                   DayInWeek.Monday
                   DayInWeek.Tuesday
                   DayInWeek.Wednesday
                   DayInWeek.Thursday
                   DayInWeek.Friday
                   DayInWeek.Saturday |]
        }
        |> AnsiConsole.Prompt

    let private dayPrompt () =
        selectionPrompt<Day> {
            title "Which day?"

            choices
                [| Day.First
                   Day.Second
                   Day.Third
                   Day.Fourth
                   Day.Fifth
                   Day.Sixth
                   Day.Seventh
                   Day.Eighth
                   Day.Ninth
                   Day.Tenth
                   Day.Eleventh
                   Day.Twelfth
                   Day.Thirteenth
                   Day.Fourteenth
                   Day.Fifteenth
                   Day.Sixteenth
                   Day.Seventeenth
                   Day.Eighteenth
                   Day.Nineteenth
                   Day.Twentieth
                   Day.Twentyfirst
                   Day.Twentysecond
                   Day.Twentythird
                   Day.Twentyfourth
                   Day.Twentyfifth
                   Day.Twentysixth
                   Day.Twentyseventh
                   Day.Twentyeighth
                   Day.Twentyninth
                   Day.Thirtieth
                   Day.Thirtyfirst |]
        }
        |> AnsiConsole.Prompt

    let private monthPrompt () =
        selectionPrompt<Month> {
            title "Which month?"

            choices
                [| Month.January
                   Month.February
                   Month.March
                   Month.April
                   Month.May
                   Month.June
                   Month.July
                   Month.August
                   Month.September
                   Month.October
                   Month.November
                   Month.December |]
        }
        |> AnsiConsole.Prompt

    let rec private recurringTypePrompt () =
        let selection =
            selectionPrompt<RecurringTypePromptSelection> {
                title "Select a recurring type:"

                choices
                    [| RecurringTypePromptSelection.EveryWeek
                       RecurringTypePromptSelection.EveryMonth
                       RecurringTypePromptSelection.EveryYear |]
            }
            |> AnsiConsole.Prompt

        match selection with
        | RecurringTypePromptSelection.EveryWeek ->
            let dayInWeek = dayInWeekPrompt ()
            RecurringType.EveryWeek dayInWeek
        | RecurringTypePromptSelection.EveryMonth ->
            let day = dayPrompt ()
            RecurringType.EveryMonth day
        | RecurringTypePromptSelection.EveryYear ->
            let month = monthPrompt ()
            let day = dayPrompt ()
            RecurringType.EveryYear (month, day)


    let handle (now: DateTime) (data: MyCalendarData) (args: RecurringEventArguments list) =
        match args with
        | [ RecurringEventArguments.Add ] ->
            let name =
                textPrompt<string> { text "What's the name of the new Recurring Event?" }
                |> AnsiConsole.Prompt

            let description =
                textPrompt<string> { text "Give it a brief description:" } |> AnsiConsole.Prompt

            let recurringType = recurringTypePrompt ()
            AnsiConsole.WriteLine $"{recurringType}"

            let isImportant =
                textPrompt<bool> {
                    text "Is it important? [grey](N/y)[/]"
                    choices [| true; false |]
                    converter (fun b -> if b then "y" else "n")
                    default_value false
                    hide_default_value
                }
                |> AnsiConsole.Prompt

            let recurringEvent =
                { Id = Guid.NewGuid()
                  Name = name
                  Description = description
                  IsImportant = isImportant
                  RecurringType = recurringType
                  CreatedAt = now
                  SoftDeleted = false }

            let newRecurringEvents = Array.append [| recurringEvent |] data.RecurringEvents
            let newData = { data with RecurringEvents = newRecurringEvents }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ RecurringEventArguments.Edit ] ->
            let recurringEvent =
                selectionPrompt<RecurringEvent> {
                    title "Select a Recurring Event you want to edit:"
                    page_size 10
                    choices (data.RecurringEvents)
                }
                |> AnsiConsole.Prompt

            let name =
                textPrompt<string> {
                    text "What's the new name of the Recurring Event?"
                    default_value recurringEvent.Name
                }
                |> AnsiConsole.Prompt

            let description =
                textPrompt<string> {
                    text "What's the new description of the Recurring Event?"
                    default_value recurringEvent.Description
                }
                |> AnsiConsole.Prompt

            let recurringType = recurringTypePrompt ()
            AnsiConsole.WriteLine $"{recurringType}"

            let isImportant =
                textPrompt<bool> {
                    text "Is it important? [grey](n/y)[/]"
                    choices [| true; false |]
                    converter (fun b -> if b then "y" else "n")
                    default_value recurringEvent.IsImportant
                }
                |> AnsiConsole.Prompt

            let updatedRecurringEvent =
                { Id = recurringEvent.Id
                  Name = name
                  Description = description
                  IsImportant = isImportant
                  RecurringType = recurringType
                  CreatedAt = now
                  SoftDeleted = false }

            let newRecurringEvents = RecurringEvent.update updatedRecurringEvent data.RecurringEvents
            let newData = { data with RecurringEvents = newRecurringEvents }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ RecurringEventArguments.Delete ] ->
            let recurringEvent =
                selectionPrompt<RecurringEvent> {
                    title "Select a Recurring Event you want to delete:"
                    page_size 10
                    choices (data.RecurringEvents)
                }
                |> AnsiConsole.Prompt

            let deleted = { recurringEvent with SoftDeleted = true }
            let newRecurringEvents = RecurringEvent.update deleted data.RecurringEvents
            let newData = { data with RecurringEvents = newRecurringEvents }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | _ -> markup { text "[red]Too many sub arguments provided![/]" } |> AnsiConsole.Write
