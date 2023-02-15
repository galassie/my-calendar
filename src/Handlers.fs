namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module Handlers =

    let handleToDoSubArgs (now: DateTime) (data: MyCalendarData) (args: ToDoSubArguments) =
        match args with
        | Add ->
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

        | Done ->
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

        | Undone ->
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

    let handle (args: Arguments) =
        let now = DateTime.Now
        let data = Storage.retrieve now

        match args with
        | Show -> Views.mainView now data |> AnsiConsole.Write

        | ToDo subArgs -> subArgs.GetAllResults() |> List.map (handleToDoSubArgs now data) |> ignore
