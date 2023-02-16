namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module Handlers =

    let handleToDoSubArgs (now: DateTime) (data: MyCalendarData) (subArgs: ToDoSubArguments list) =
        match subArgs with
        | [ Add ] ->
            let name =
                textPrompt<string> { text "What's the new of the new ToDo?" }
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

        | [ Done ] ->
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

        | [ Undone ] ->
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

        | _ -> markup { text "[red]Too many sub arguments provided![/]" } |> AnsiConsole.Write

    let handle (args: Arguments list) =
        let now = DateTime.Now
        let data = Storage.retrieve now

        match args with
        | [ Show ] -> Views.mainView now data |> AnsiConsole.Write

        | [ ToDo subArgs ] ->
            let subArgs = subArgs.GetAllResults()
            handleToDoSubArgs now data subArgs

        | _ -> markup { text "[red]Too many arguments provided![/]" } |> AnsiConsole.Write
