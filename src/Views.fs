namespace MyCalendar

open System
open FsSpectre
open Spectre.Console
open Spectre.Console.Rendering

[<RequireQualifiedAccess>]
module Views =

    let calendar (now: DateTime) =
        grid {
            number_of_columns 1

            row
                [| calendar {
                       events [| calendarEvent { date_time now } |]
                       highlight_style (style { foreground Color.Green })
                   } |]

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

    let nextEvents (now: DateTime) (recurringEvents: RecurringEvent array) (events: Event array) =
        let nextEvents = Event.nextEvents now events
        let toShow: IRenderable array =
            nextEvents
            |> Array.map (fun event ->
                let str = Event.toString event

                if event.IsImportant then
                    markup { text $"· [yellow]{str}[/]" }
                else
                    markup { text $"· {str}" })

        panel {
            expand
            width 32
            height 32
            header_text "Next events"
            content_renderable (rows { items_renderable toShow })
        }

    let mainView (now: DateTime) (data: MyCalendarData) =
        let todos = ToDo.extractForView now data.ToDos

        grid {
            number_of_columns 3
            empty_row

            row
                [| calendar now
                   todo todos
                   nextEvents now data.RecurringEvents data.Events |]

            empty_row
        }
