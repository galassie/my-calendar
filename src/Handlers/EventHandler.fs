namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module EventHandler =

    let handle (now: DateTime) (data: MyCalendarData) (args: EventArguments list) =
        match args with
        | [ EventArguments.Add ] ->
            let name =
                textPrompt<string> { text "What's the name of the new Event?" }
                |> AnsiConsole.Prompt

            let description =
                textPrompt<string> { text "Give it a brief description:" } |> AnsiConsole.Prompt

            let onlyDate =
                textPrompt<string> {
                    text "When? [grey](yyyy-mm-dd)[/]"

                    validator (fun input ->
                        match OnlyDate.TryParse(input) with
                        | Some _ -> ValidationResult.Success()
                        | None -> ValidationResult.Error "[red]The date inserted is not valid![/]")
                }
                |> AnsiConsole.Prompt
                |> OnlyDate.TryParse
                |> Option.defaultValue
                    { Year = now.Year
                      Month = now.Month
                      Day = now.Day }

            let isImportant =
                textPrompt<bool> {
                    text "Is it important? [grey](N/y)[/]"
                    choices [| true; false |]
                    converter (fun b -> if b then "y" else "n")
                    default_value false
                    hide_default_value
                }
                |> AnsiConsole.Prompt

            let event =
                { Id = Guid.NewGuid()
                  Name = name
                  Description = description
                  IsImportant = isImportant
                  When = onlyDate
                  CreatedAt = now
                  SoftDeleted = false }

            let newEvents = Array.append [| event |] data.Events
            let newData = { data with Events = newEvents }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ EventArguments.Edit ] ->
            let event =
                selectionPrompt<Event> {
                    title "Select a Event you want to edit:"
                    page_size 10
                    choices (Event.nextEvents false Constants.maxCount now data.Events)
                }
                |> AnsiConsole.Prompt

            let name =
                textPrompt<string> {
                    text "What's the new name of the Event?"
                    default_value event.Name
                }
                |> AnsiConsole.Prompt

            let description =
                textPrompt<string> {
                    text "What's the new description of the Event?"
                    default_value event.Description
                }
                |> AnsiConsole.Prompt

            let onlyDate =
                textPrompt<string> {
                    text "What's the new date? [grey](yyyy-mm-dd)[/]"

                    validator (fun input ->
                        match OnlyDate.TryParse(input) with
                        | Some _ -> ValidationResult.Success()
                        | None -> ValidationResult.Error "[red]The date inserted is not valid![/]")

                    default_value (event.When.ToString())
                }
                |> AnsiConsole.Prompt
                |> OnlyDate.TryParse
                |> Option.defaultValue
                    { Year = now.Year
                      Month = now.Month
                      Day = now.Day }

            let isImportant =
                textPrompt<bool> {
                    text "Is it important? [grey](n/y)[/]"
                    choices [| true; false |]
                    converter (fun b -> if b then "y" else "n")
                    default_value event.IsImportant
                }
                |> AnsiConsole.Prompt

            let updatedEvent =
                { event with
                    Name = name
                    Description = description
                    When = onlyDate
                    IsImportant = isImportant }

            let newEvents = Event.update updatedEvent data.Events
            let newData = { data with Events = newEvents }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ EventArguments.Delete ] ->
            let event =
                selectionPrompt<Event> {
                    title "Select an Event you want to delete:"
                    page_size 10
                    choices (Event.nextEvents false Constants.maxCount now data.Events)
                }
                |> AnsiConsole.Prompt

            let deleted = { event with SoftDeleted = true }
            let newEvents = Event.update deleted data.Events
            let newData = { data with Events = newEvents }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | _ -> markup { text "Wrong sub arguments provided!\nTry `my-calendar event --help` for more information.\n" } |> AnsiConsole.Write
