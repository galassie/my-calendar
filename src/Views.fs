namespace MyCalendar

open System
open FsSpectre
open Spectre.Console.Rendering

[<RequireQualifiedAccess>]
module Views =

    let calendar (now: DateTime) =
        grid {
            number_of_columns 1
            row [| calendar { events [| calendarEvent { date_time now } |] } |]

            row
                [| text {
                       text $"{now.Day} {now.DayOfWeek}"
                       centered
                   } |]
        }

    let todo (todos: ToDo array) =
        let toShow: IRenderable array =
            todos
            |> Array.map (fun todo ->
                let str = ToDo.toString todo

                if Option.isSome todo.MarkedDoneAt then
                    markup { text $"· [strikethrough]{str}[/]" }
                else
                    markup { text $"· {str}" })

        panel {
            expand
            width 32
            height 32
            header_text "Todo"
            content_renderable (rows { items_renderable toShow })
        }

    let eventsOfTheMonth (now: DateTime) (recurringEvents: RecurringEvent array) (events: Event array) =
        panel {
            expand
            width 32
            height 32
            header_text "Events of the month"
            content_renderable (rows { items_renderable [||] })
        }

    let mainView (now: DateTime) (data: MyCalendarData) =
        let todos = ToDo.extractForView now data.ToDos

        grid {
            number_of_columns 3
            empty_row

            row [| calendar now; todo todos; eventsOfTheMonth now data.RecurringEvents data.Events |]

            empty_row
        }
