namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

[<RequireQualifiedAccess>]
module MainHandler =

    let handle (args: Arguments list) =
        let now = DateTime.Now
        let data = Storage.retrieve ()

        match args with
        | [ Show ] -> Views.mainView now data |> AnsiConsole.Write

        | [ ToDo subArgs ] ->
            let subArgs = subArgs.GetAllResults()
            ToDoHandler.handle now data subArgs

        | [ Event subArgs ] ->
            let subArgs = subArgs.GetAllResults()
            EventHandler.handle now data subArgs

        | _ -> markup { text "[red]Too many arguments provided![/]" } |> AnsiConsole.Write
