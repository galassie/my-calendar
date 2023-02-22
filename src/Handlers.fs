namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module Handlers =

    let handleToDoSubArgs (now: DateTime) (data: MyCalendarData) (subArgs: ToDoSubArguments list) =
        match subArgs with
        | [ ToDoSubArguments.Add ] ->
            let name =
                textPrompt<string> { text "What's the name of the new ToDo?" }
                |> AnsiConsole.Prompt

            let description =
                textPrompt<string> { text "Give it a brief description:" } |> AnsiConsole.Prompt

            let todo =
                { Id = Guid.NewGuid()
                  Name = name
                  Description = description
                  CreatedAt = now
                  MarkedDoneAt = None
                  SoftDeleted = false }

            let newToDos = Array.append [| todo |] data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store now newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoSubArguments.Edit ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to edit:"
                    page_size 10
                    choices (ToDo.active data.ToDos)
                    converter ToDo.toString
                }
                |> AnsiConsole.Prompt

            let name =
                textPrompt<string> {
                    text "What's the new name of the ToDo?"
                    default_value todo.Name
                }
                |> AnsiConsole.Prompt

            let description =
                textPrompt<string> {
                    text "What's the new description of the ToDo?"
                    default_value todo.Description
                }
                |> AnsiConsole.Prompt

            let updatedTodo =
                { todo with
                    Name = name
                    Description = description }

            let newToDos = ToDo.update updatedTodo data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store now newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoSubArguments.Done ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to mark as done:"
                    page_size 10
                    choices (ToDo.active data.ToDos)
                    converter ToDo.toString
                }
                |> AnsiConsole.Prompt

            let markedDone = { todo with MarkedDoneAt = Some now }
            let newToDos = ToDo.update markedDone data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store now newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoSubArguments.Undone ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to undo:"
                    page_size 10
                    choices (ToDo.markedDone data.ToDos)
                    converter ToDo.toString
                }
                |> AnsiConsole.Prompt

            let unmarkedDone = { todo with MarkedDoneAt = None }
            let newToDos = ToDo.update unmarkedDone data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store now newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoSubArguments.Delete ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to delete:"
                    page_size 10
                    choices (ToDo.deletable data.ToDos)
                    converter ToDo.toString
                }
                |> AnsiConsole.Prompt

            let deleted = { todo with SoftDeleted = true }
            let newToDos = ToDo.update deleted data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store now newData

            Views.mainView now newData |> AnsiConsole.Write

        | _ -> markup { text "[red]Too many sub arguments provided![/]" } |> AnsiConsole.Write

    let handleEventSubArgs (now: DateTime) (data: MyCalendarData) (subArgs: EventSubArguments list) =
        match subArgs with
        | [ EventSubArguments.Add ] ->
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

    let handle (args: Arguments list) =
        let now = DateTime.Now
        let data = Storage.retrieve now

        match args with
        | [ Show ] -> Views.mainView now data |> AnsiConsole.Write

        | [ ToDo subArgs ] ->
            let subArgs = subArgs.GetAllResults()
            handleToDoSubArgs now data subArgs

        | [ Event subArgs ] ->
            let subArgs = subArgs.GetAllResults()
            handleEventSubArgs now data subArgs

        | _ -> markup { text "[red]Too many arguments provided![/]" } |> AnsiConsole.Write
