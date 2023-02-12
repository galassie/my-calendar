namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

type Handlers =

    static member private NextEventsPanel() =
        panel {
            expand
            width 32
            header_text "Next events"
            content_renderable (rows { items_renderable [| text { text "hello" }; text { text "hello" } |] })
        }

    static member Handle(args: Arguments) =
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
