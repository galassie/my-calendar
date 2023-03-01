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
                       header_style (style { foreground Color.Blue })
                   } |]

            row
                [| markup {
                       text $"[blue]{now.Day} {now.DayOfWeek}[/]"
                       centered
                   } |]
        }

    let todo (todos: ToDo array) =
        let toShow: IRenderable array =
            todos
            |> Array.map (fun todo ->
                let str = ToDo.toString todo

                if Option.isSome todo.MarkedDoneAt then
                    markup { text $"[strikethrough grey]➢ {str}[/]" }
                else
                    markup { text $"➢ {str}" })

        panel {
            expand
            width 32
            height 32
            header_text "[bold]Todo[/]"
            border_color Color.Blue
            content_renderable (rows { items_renderable toShow })
        }

    let private todayEvent (now: DateTime) (event: Event) =
        now.Day = event.When.Day && now.Month = event.When.Month && now.Year = event.When.Year

    let nextEvents (now: DateTime) (recurringEvents: RecurringEvent array) (events: Event array) =
        let nextEvents = Event.nextEvents now events
        let toShow: IRenderable array =
            nextEvents
            |> Array.map (fun event ->
                let str = Event.toString event

                let isImportant = if event.IsImportant then " [red]⚠[/]" else ""

                if todayEvent now event then
                    markup { text $"➢ [green]{str}[/]{isImportant}" }
                else
                    markup { text $"➢ {str}{isImportant}" })

        panel {
            expand
            width 32
            height 32
            header_text "[bold]Next events[/]"
            border_color Color.Blue
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
