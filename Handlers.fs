namespace MyCalendar

open System
open Spectre.Console
open FsSpectre

type Handlers =

    static member private TodoPanel() =
        panel {
          expand
          width 32
          header_text "Todo"
          content_renderable (
            rows {
              items_renderable [|
                text { text "hello" }
                text { text "hello" }
              |]
            }
          )
        }

    static member private NextEventsPanel() =
        panel {
          expand
          width 32
          header_text "Next events"
          content_renderable (
            rows {
              items_renderable [|
                text { text "hello" }
                text { text "hello" }
              |]
            }
          )
        }

    static member Handle(args: Arguments) =
        let now = DateTime.Now

        match args with
        | Show ->
            grid {
                number_of_columns 3
                empty_row
                row [| calendar { events [||] }; Handlers.TodoPanel (); Handlers.NextEventsPanel () |]

                row
                    [| text {
                           text $"{now.Day} {now.DayOfWeek}"
                           centered
                       }
                       panel { width 32 }
                       panel { width 32 } |]

                empty_row
            }
            |> AnsiConsole.Write
