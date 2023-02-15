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
                textPrompt<string> {
                    text "What's the new of the new ToDo?"
                } |> AnsiConsole.Prompt
            
            let description =
                textPrompt<string> {
                    text "Give it a brief description:"
                } |> AnsiConsole.Prompt
            
            let todo = 
                { Name = name
                  Description = description 
                  CreatedAt = now
                  MarkedDoneAt =  None
                  SoftDeleted = false }
            
            let newData = { data with ToDos = Array.append [| todo |] data.ToDos }

            Storage.store now newData
            

        | Done -> AnsiConsole.WriteLine "test"

    let handle (args: Arguments) =
        let now = DateTime.Now
        let data = Storage.retrieve now

        match args with
        | Show ->
            grid {
                number_of_columns 3
                empty_row

                row
                    [| Views.calendar now
                       Views.todo now data.ToDos
                       Views.nextEvents now data.RecurringEvents data.Events |]

                empty_row
            }
            |> AnsiConsole.Write
        
        | ToDo subArgs ->
            subArgs.GetAllResults() |> List.map (handleToDoSubArgs now data) |> ignore


            