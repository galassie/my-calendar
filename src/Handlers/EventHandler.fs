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
                        | Some _ -> ValidationResult.Success ()
                        | None -> ValidationResult.Error "The date inserted is not valid!" ) } 
                |> AnsiConsole.Prompt
                |> OnlyDate.TryParse
                |> Option.defaultValue { Year = now.Year; Month = now.Month; Day = now.Day }

            let isImportant =
                textPrompt<bool> { 
                    text "Is it important? [grey](N/y)[/]" 
                    choices [| true; false |]
                    converter (fun b -> if b then "y" else "n")
                    default_value false
                } |> AnsiConsole.Prompt

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

            Storage.store now newData

            Views.mainView now newData |> AnsiConsole.Write

        | _ -> markup { text "[red]Too many sub arguments provided![/]" } |> AnsiConsole.Write