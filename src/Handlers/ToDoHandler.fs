namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module ToDoHandler =

    let handle (now: DateTime) (data: MyCalendarData) (args: ToDoArguments list) =
        match args with
        | [ ToDoArguments.Add ] ->
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

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoArguments.Edit ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to edit:"
                    page_size 10
                    choices (ToDo.active data.ToDos)
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

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoArguments.Done ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to mark as done:"
                    page_size 10
                    choices (ToDo.active data.ToDos)
                }
                |> AnsiConsole.Prompt

            let markedDone = { todo with MarkedDoneAt = Some now }
            let newToDos = ToDo.update markedDone data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoArguments.Undone ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to undo:"
                    page_size 10
                    choices (ToDo.markedDone data.ToDos)
                }
                |> AnsiConsole.Prompt

            let unmarkedDone = { todo with MarkedDoneAt = None }
            let newToDos = ToDo.update unmarkedDone data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | [ ToDoArguments.Delete ] ->
            let todo =
                selectionPrompt<ToDo> {
                    title "Select a ToDo you want to delete:"
                    page_size 10
                    choices (ToDo.deletable data.ToDos)
                }
                |> AnsiConsole.Prompt

            let deleted = { todo with SoftDeleted = true }
            let newToDos = ToDo.update deleted data.ToDos
            let newData = { data with ToDos = newToDos }

            Storage.store newData

            Views.mainView now newData |> AnsiConsole.Write

        | _ -> markup { text "[red]Too many sub arguments provided![/]" } |> AnsiConsole.Write